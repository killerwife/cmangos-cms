using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data.Dto.Srp6
{
    public enum ChallengeFlags
    {
        SecurityFlagAuthenticator = 0x04,
    }

    public class ChallengeResponseDto
    {
        public byte[] HostPublicEphemeral { get; set; } = new byte[32];
        public byte[] GeneratorModulo { get; set; } = new byte[32];
        public byte[] Prime { get; set; } = new byte[32];
        public byte[] Salt { get; set; } = new byte[32];
        public ChallengeFlags Flags { get; set; }
    }
}
