using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PitchDetector
{
    /// <summary>
    /// Infinite impulse response filter (old style analog filters)
    /// </summary>
    class IIRFilter
    {
        /// <summary>
        /// The type of filter
        /// </summary>
        public enum FilterType
        {
            None = 0,
            LP,
            HP,
            BP
        }

        /// <summary>
        /// The filter prototype
        /// </summary>
        public enum ProtoType
        {
            None = 0,
            Butterworth,
            Chebyshev,
        }

        const int kHistMask = 31;
        const int kHistSize = 32;

        private int _order;
        private ProtoType _protoType;
        private FilterType _filterType;

        private float _fp1;
        private float _fp2;
        private float _fN;
        private float _ripple;
        private float _sampleRate;
        private double[] _reals;
        private double[] _imags;
        private double[] _zs;
        private double[] _aCoeffs;
        private double[] _bCoeffs;
        private double[] _inHistories;
        private double[] _outHistories;
        private int _histIdxs;
        private bool _isInvertDenormal;

        public IIRFilter()
        {
        }

        /// <summary>
        /// Returns true if all the filter parameters are valid
        /// </summary>
        public bool FilterValid
        {
            get
            {
                if (_order < 1 || _order > 16 ||
                    _protoType == ProtoType.None ||
                    _filterType == FilterType.None ||
                    _sampleRate <= 0.0f ||
                    _fN <= 0.0f)
                    return false;

                switch (_filterType)
                {
                    case FilterType.LP:
                        if (_fp2 <= 0.0f)
                            return false;
                        break;

                    case FilterType.BP:
                        if (_fp1 <= 0.0f || _fp2 <= 0.0f || _fp1 >= _fp2)
                            return false;
                        break;

                    case FilterType.HP:
                        if (_fp1 <= 0.0f)
                            return false;
                        break;
                }

                // For bandpass, the order must be even
                if (_filterType == FilterType.BP && (_order & 1) != 0)
                    return false;

                return true;
            }
        }

        /// <summary>
        /// Set the filter prototype
        /// </summary>
        public ProtoType Proto
        {
            get { return _protoType; }

            set
            {
                _protoType = value;
                Design();
            }
        }

        /// <summary>
        /// Set the filter type
        /// </summary>
        public FilterType Type
        {
            get { return _filterType; }

            set
            {
                _filterType = value;
                Design();
            }
        }

        public int Order
        {
            get { return _order; }

            set
            {
                _order = Math.Min(16, Math.Max(1, Math.Abs(value)));

                if (_filterType == FilterType.BP && Odd(_order))
                    _order++;

                Design();
            }
        }

        public float SampleRate
        {
            get { return _sampleRate; }

            set
            {
                _sampleRate = value;
                _fN = 0.5f * _sampleRate;
                Design();
            }
        }

        public float FreqLow
        {
            get { return _fp1; }

            set
            {
                _fp1 = value;
                Design();
            }
        }

        public float FreqHigh
        {
            get { return _fp2; }

            set
            {
                _fp2 = value;
                Design();
            }
        }

        public float Ripple
        {
            get { return _ripple; }

            set
            {
                _ripple = value;
                Design();
            }
        }

        /// <summary>
        /// Returns true if n is odd
        /// </summary>
        /// <param name="n"></param>
        /// <returns></returns>
        private bool Odd(int n)
        {
            return (n & 1) == 1;
        }

        /// <summary>
        /// Square
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private float Sqr(float value)
        {
            return value * value;
        }

        /// <summary>
        /// Square
        /// </summary>
        /// <param name="f"></param>
        /// <returns></returns>
        private double Sqr(double value)
        {
            return value * value;
        }

        /// <summary>
        /// Determines poles and zeros of IIR filter
        /// based on bilinear transform method
        /// </summary>
        private void LocatePolesAndZeros()
        {
            _reals = new double[_order + 1];
            _imags = new double[_order + 1];
            _zs = new double[_order + 1];
            double ln10 = Math.Log(10.0);

            // Butterworth, Chebyshev parameters
            int n = _order;

            if (_filterType == FilterType.BP)
                n = n / 2;

            int ir = n % 2;
            int n1 = n + ir;
            int n2 = (3 * n + ir) / 2 - 1;
            double f1;

            switch (_filterType)
            {
                case FilterType.LP:
                    f1 = _fp2;
                    break;

                case FilterType.HP:
                    f1 = _fN - _fp1;
                    break;

                case FilterType.BP:
                    f1 = _fp2 - _fp1;
                    break;

                default:
                    f1 = 0.0;
                    break;
            }

            double tanw1 = Math.Tan(0.5 * Math.PI * f1 / _fN);
            double tansqw1 = Sqr(tanw1);

            // Real and Imaginary parts of low-pass poles
            double t, a = 1.0, r = 1.0, i = 1.0;

            for (int k = n1; k <= n2; k++)
            {
                t = 0.5 * (2 * k + 1 - ir) * Math.PI / (double)n;

                switch (_protoType)
                {
                    case ProtoType.Butterworth:
                        double b3 = 1.0 - 2.0 * tanw1 * Math.Cos(t) + tansqw1;
                        r = (1.0 - tansqw1) / b3;
                        i = 2.0 * tanw1 * Math.Sin(t) / b3;
                        break;

                    case ProtoType.Chebyshev:
                        double d = 1.0 - Math.Exp(-0.05 * _ripple * ln10);
                        double e = 1.0 / Math.Sqrt(1.0 / Sqr(1.0 - d) - 1.0);
                        double x = Math.Pow(Math.Sqrt(e * e + 1.0) + e, 1.0 / (double)n);
                        a = 0.5 * (x - 1.0 / x);
                        double b = 0.5 * (x + 1.0 / x);
                        double c3 = a * tanw1 * Math.Cos(t);
                        double c4 = b * tanw1 * Math.Sin(t);
                        double c5 = Sqr(1.0 - c3) + Sqr(c4);
                        r = 2.0 * (1.0 - c3) / c5 - 1.0;
                        i = 2.0 * c4 / c5;
                        break;
                }

                int m = 2 * (n2 - k) + 1;
                _reals[m + ir] = r;
                _imags[m + ir] = Math.Abs(i);
                _reals[m + ir + 1] = r;
                _imags[m + ir + 1] = -Math.Abs(i);
            }

            if (Odd(n))
            {
                if (_protoType == ProtoType.Butterworth)
                    r = (1.0 - tansqw1) / (1.0 + 2.0 * tanw1 + tansqw1);

                if (_protoType == ProtoType.Chebyshev)
                    r = 2.0 / (1.0 + a * tanw1) - 1.0;

                _reals[1] = r;
                _imags[1] = 0.0;
            }

            switch (_filterType)
            {
                case FilterType.LP:
                    for (int m = 1; m <= n; m++)
                        _zs[m] = -1.0;
                    break;

                case FilterType.HP:
                    // Low-pass to high-pass transformation
                    for (int m = 1; m <= n; m++)
                    {
                        _reals[m] = -_reals[m];
                        _zs[m] = 1.0;
                    }
                    break;

                case FilterType.BP:
                    // Low-pass to bandpass transformation
                    for (int m = 1; m <= n; m++)
                    {
                        _zs[m] = 1.0;
                        _zs[m + n] = -1.0;
                    }

                    double f4 = 0.5 * Math.PI * _fp1 / _fN;
                    double f5 = 0.5 * Math.PI * _fp2 / _fN;
                    double aa = Math.Cos(f4 + f5) / Math.Cos(f5 - f4);
                    double aR, aI, h1, h2, p1R, p2R, p1I, p2I;

                    for (int m1 = 0; m1 <= (_order - 1) / 2; m1++)
                    {
                        int m = 1 + 2 * m1;
                        aR = _reals[m];
                        aI = _imags[m];

                        if (Math.Abs(aI) < 0.0001)
                        {
                            h1 = 0.5 * aa * (1.0 + aR);
                            h2 = Sqr(h1) - aR;
                            if (h2 > 0.0)
                            {
                                p1R = h1 + Math.Sqrt(h2);
                                p2R = h1 - Math.Sqrt(h2);
                                p1I = 0.0;
                                p2I = 0.0;
                            }
                            else
                            {
                                p1R = h1;
                                p2R = h1;
                                p1I = Math.Sqrt(Math.Abs(h2));
                                p2I = -p1I;
                            }
                        }
                        else
                        {
                            double fR = aa * 0.5 * (1.0 + aR);
                            double fI = aa * 0.5 * aI;
                            double gR = Sqr(fR) - Sqr(fI) - aR;
                            double gI = 2 * fR * fI - aI;
                            double sR = Math.Sqrt(0.5 * Math.Abs(gR + Math.Sqrt(Sqr(gR) + Sqr(gI))));
                            double sI = gI / (2.0 * sR);
                            p1R = fR + sR;
                            p1I = fI + sI;
                            p2R = fR - sR;
                            p2I = fI - sI;
                        }

                        _reals[m] = p1R;
                        _reals[m + 1] = p2R;
                        _imags[m] = p1I;
                        _imags[m + 1] = p2I;
                    }

                    if (Odd(n))
                    {
                        _reals[2] = _reals[n + 1];
                        _imags[2] = _imags[n + 1];
                    }

                    for (int k = n; k >= 1; k--)
                    {
                        int m = 2 * k - 1;
                        _reals[m] = _reals[k];
                        _reals[m + 1] = _reals[k];
                        _imags[m] = Math.Abs(_imags[k]);
                        _imags[m + 1] = -Math.Abs(_imags[k]);
                    }

                    break;
            }
        }

        /// <summary>
        /// Calculate all the values
        /// </summary>
        public void Design()
        {
            if (!this.FilterValid)
                return;

            _aCoeffs = new double[_order + 1];
            _bCoeffs = new double[_order + 1];
            _inHistories = new double[kHistSize];
            _outHistories = new double[kHistSize];

            double[] newA = new double[_order + 1];
            double[] newB = new double[_order + 1];

            // Find filter poles and zeros
            LocatePolesAndZeros();

            // Compute filter coefficients from pole/zero values
            _aCoeffs[0] = 1.0;
            _bCoeffs[0] = 1.0;

            for (int i = 1; i <= _order; i++)
            {
                _aCoeffs[i] = 0.0;
                _bCoeffs[i] = 0.0;
            }

            int k = 0;
            int n = _order;
            int pairs = n / 2;

            if (Odd(_order))
            {
                // First subfilter is first order
                _aCoeffs[1] = -_zs[1];
                _bCoeffs[1] = -_reals[1];
                k = 1;
            }

            for (int p = 1; p <= pairs; p++)
            {
                int m = 2 * p - 1 + k;
                double alpha1 = -(_zs[m] + _zs[m + 1]);
                double alpha2 = _zs[m] * _zs[m + 1];
                double beta1 = -2.0 * _reals[m];
                double beta2 = Sqr(_reals[m]) + Sqr(_imags[m]);

                newA[1] = _aCoeffs[1] + alpha1 * _aCoeffs[0];
                newB[1] = _bCoeffs[1] + beta1 * _bCoeffs[0];

                for (int i = 2; i <= n; i++)
                {
                    newA[i] = _aCoeffs[i] + alpha1 * _aCoeffs[i - 1] + alpha2 * _aCoeffs[i - 2];
                    newB[i] = _bCoeffs[i] + beta1 * _bCoeffs[i - 1] + beta2 * _bCoeffs[i - 2];
                }

                for (int i = 1; i <= n; i++)
                {
                    _aCoeffs[i] = newA[i];
                    _bCoeffs[i] = newB[i];
                }
            }

            // Ensure the filter is normalized
            FilterGain(1000);
        }

        /// <summary>
        /// Reset the history buffers
        /// </summary>
        public void Reset()
        {
            if (_inHistories != null)
                _inHistories.Clear();

            if (_outHistories != null)
                _outHistories.Clear();

            _histIdxs = 0;
        }

        /// <summary>
        /// Reset the filter, and fill the appropriate history buffers with the specified value
        /// </summary>
        /// <param name="historyValue"></param>
        public void Reset(double startValue)
        {
            _histIdxs = 0;

            if (_inHistories == null || _outHistories == null)
                return;

            _inHistories.Fill(startValue);

            if (_inHistories != null)
            {
                switch (_filterType)
                {
                    case FilterType.LP:
                        _outHistories.Fill(startValue);
                        break;

                    default:
                        _outHistories.Clear();
                        break;
                }
            }
        }

        /// <summary>
        /// Apply the filter to the buffer
        /// </summary>
        /// <param name="bufIn"></param>
        public void FilterBuffer(float[] srcBuf, long srcPos, float[] dstBuf, long dstPos, long nLen)
        {
            const double kDenormal = 0.000000000000001;
            double denormal = _isInvertDenormal ? -kDenormal : kDenormal;
            _isInvertDenormal = !_isInvertDenormal;

            for (int sampleIdx = 0; sampleIdx < nLen; sampleIdx++)
            {
                double sum = 0.0f;

                _inHistories[_histIdxs] = srcBuf[srcPos + sampleIdx] + denormal;

                for (int idx = 0; idx < _aCoeffs.Length; idx++)
                    sum += _aCoeffs[idx] * _inHistories[(_histIdxs - idx) & kHistMask];

                for (int idx = 1; idx < _bCoeffs.Length; idx++)
                    sum -= _bCoeffs[idx] * _outHistories[(_histIdxs - idx) & kHistMask];

                _outHistories[_histIdxs] = sum;
                _histIdxs = (_histIdxs + 1) & kHistMask;
                dstBuf[dstPos + sampleIdx] = (float)sum;
            }
        }

        public float FilterSample(float inVal)
        {
            double sum = 0.0f;

            _inHistories[_histIdxs] = inVal;

            for (int idx = 0; idx < _aCoeffs.Length; idx++)
                sum += _aCoeffs[idx] * _inHistories[(_histIdxs - idx) & kHistMask];

            for (int idx = 1; idx < _bCoeffs.Length; idx++)
                sum -= _bCoeffs[idx] * _outHistories[(_histIdxs - idx) & kHistMask];

            _outHistories[_histIdxs] = sum;
            _histIdxs = (_histIdxs + 1) & kHistMask;

            return (float)sum;
        }

        /// <summary>
        /// Get the gain at the specified number of frequency points
        /// </summary>
        /// <param name="freqPoints"></param>
        /// <returns></returns>
        public float[] FilterGain(int freqPoints)
        {
            // Filter gain at uniform frequency intervals
            float[] g = new float[freqPoints];
            double theta, s, c, sac, sas, sbc, sbs;
            float gMax = -100.0f;
            float sc = 10.0f / (float)Math.Log(10.0f);
            double t = Math.PI / (freqPoints - 1);

            for (int i = 0; i < freqPoints; i++)
            {
                theta = i * t;

                if (i == 0)
                    theta = Math.PI * 0.0001;

                if (i == freqPoints - 1)
                    theta = Math.PI * 0.9999;

                sac = 0.0f;
                sas = 0.0f;
                sbc = 0.0f;
                sbs = 0.0f;

                for (int k = 0; k <= _order; k++)
                {
                    c = Math.Cos(k * theta);
                    s = Math.Sin(k * theta);
                    sac += c * _aCoeffs[k];
                    sas += s * _aCoeffs[k];
                    sbc += c * _bCoeffs[k];
                    sbs += s * _bCoeffs[k];
                }

                g[i] = sc * (float)Math.Log((Sqr(sac) + Sqr(sas)) / (Sqr(sbc) + Sqr(sbs)));
                gMax = Math.Max(gMax, g[i]);
            }

            // Normalize to 0 dB maximum gain
            for (int i = 0; i < freqPoints; i++)
                g[i] -= gMax;

            // Normalize numerator (a) coefficients
            float normFactor = (float)Math.Pow(10.0, -0.05 * gMax);

            for (int i = 0; i <= _order; i++)
                _aCoeffs[i] *= normFactor;

            return g;
        }
    }
}
