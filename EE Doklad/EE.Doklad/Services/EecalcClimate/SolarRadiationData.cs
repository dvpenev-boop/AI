namespace EE.Doklad.Services.EecalcClimate
{
    public sealed record SolarRadiationData(double N, double E, double S, double W, double H)
    {
        public double NE => (N + E) / 2.0;
        public double SE => (S + E) / 2.0;
        public double SW => (S + W) / 2.0;
        public double NW => (N + W) / 2.0;
    }
}
