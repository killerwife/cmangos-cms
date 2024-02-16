namespace Data.Dto
{
    public class ProofDto
    {
        public byte[] Answer { get; set; } = new byte[32];
        public byte[] M1 { get; set; } = new byte[20];
        public byte[] CrcHash { get; set; } = new byte[20];
        public byte[] Pin { get; set; } = new byte[6]; 
    }
}
