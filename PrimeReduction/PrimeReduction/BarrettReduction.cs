namespace PrimeReduction
{
    // Barret Reduction Algorithm for ModPow operation
    // https://www.nayuki.io/page/barrett-reduction-algorithm
    internal static class BarrettReduction
    {
        public static uint ModPow(uint baseN, uint e, uint mod)
        {
            var k = CeilLog2(mod);
            var r = CaclRFactor(mod, k);
            uint d = 1;

            baseN = baseN % mod;
            if((e & 1) == 1)
            {
                d = baseN;
            }

            while((e >>= 1) != 0)
            {
                baseN = ModMul(baseN, baseN, mod, r, k);
                if((e & 1) == 1)
                {
                    d = ModMul(d, baseN, mod, r, k);
                }
            }
            return d;
        }

        private static uint ModMul(uint a, uint b, uint mod, uint r, uint k)
        {
            ulong p = a;
            p *= b;

            if(p < mod)
                return (uint)p;

            var t = p >> (int)k;
            t *= r;
            t >>= (int)(k - 1);
            t *= mod;
            p -= t;

            int ct = 4;
            while(p >= mod && ct-- != 0)
            {
                p -= mod;
            }
            return (uint)p;
        }

        private static uint CeilLog2(uint v)
        {
            uint s = 0;

            while(v > 0)
            {
                s++;
                v >>= 1;
            }
            return s;
        }

        private static uint CaclRFactor(uint n, uint k)
        {
            //2^(k*2 - 1) / n
            ulong d = 1;
            d <<= (int)((k << 1) - 1);
            return (uint)(d / n);
        }
    }
}
