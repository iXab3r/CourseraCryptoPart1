using System.Net;

namespace CryptoWeek4
{
    public interface IPaddingOracle
    {
        byte[] GetCipherText();

        HttpStatusCode QueryOracle(byte[] _cipherTextBytes);
    }
}