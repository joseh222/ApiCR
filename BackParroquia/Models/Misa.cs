namespace BackParroquia.Models
{
    public class Misa
    {
        public int IdMisa { get; set; }
        public TipoMisa? TipoMisa { get; set; }
        public MotivoMisa? MotivoMisa { get; set; }
        public string? Motivo { get; set; }
        //public List<Nombres>? ListNombres { get; set; }
        public DateTime? FhMisa { get; set; }
        public decimal? Donacion { get; set; }
        public string? Observaciones { get; set; }
        public bool FlgMisaPersonal { get; set; }
        public DateTime? FhCreacion { get; set; }
        public DateTime? FhActualizacion { get; set; }
        public bool FlgEliminado { get; set; }
        public DateTime? DateMass { get; set; }
        public TimeSpan? HoraMass { get; set; }
        //propiedades para usar en el mudtable
        public string? StrFhMisa { get; set; }
        public bool ShowDetails { get; set; } //para detalle del registro
    }
}
