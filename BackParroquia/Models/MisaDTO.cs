namespace BackParroquia.Models
{
    public class MisaDTO
    {
        public int IdMisa { get; set; }
        public TipoMisa? TipoMisa { get; set; }
        public MotivoMisa? MotivoMisa { get; set; }
        public string? Motivo { get; set; }
        public List<Nombres>? ListNombres { get; set; }
        public DateTime? FhMisa { get; set; }
        public decimal? Donacion { get; set; }
        public string? Observaciones { get; set; }
        public bool FlgMisaPersonal { get; set; }
        public DateTime? DateMass { get; set; }
        public TimeSpan? HoraMass { get; set; }
    }
}
