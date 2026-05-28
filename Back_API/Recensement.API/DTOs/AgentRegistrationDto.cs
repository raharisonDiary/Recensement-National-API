namespace Recensement.API.DTOs
{
    public class AgentRegistrationDto
    {
        public string Cin { get; set; } = string.Empty;
        public string Nom { get; set; } = string.Empty;
        public string Prenom { get; set; } = string.Empty;
        public string Telephone { get; set; } = string.Empty;
        public string Adresse { get; set; } = string.Empty;
        public string RegionAssigned { get; set; } = string.Empty;
    }
}