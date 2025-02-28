using System;

namespace Zyh.Common.Security
{
    /// <summary>
    /// CRC-32 with reversed data and unreversed output
    /// </summary>
    /// <remarks>
    /// Generate a table for a byte-wise 32-bit CRC calculation on the polynomial:
    /// x^32+x^26+x^23+x^22+x^16+x^12+x^11+x^10+x^8+x^7+x^5+x^4+x^2+x^1+x^0.
    ///
    /// Polynomials over GF(2) are represented in binary, one bit per coefficient,
    /// with the lowest powers in the most significant bit.  Then adding polynomials
    /// is just exclusive-or, and multiplying a polynomial by x is a right shift by
    /// one.  If we call the above polynomial p, and represent a byte as the
    /// polynomial q, also with the lowest power in the most significant bit (so the
    /// byte 0xb1 is the polynomial x^7+x^3+x+1), then the CRC is (q*x^32) mod p,
    /// where a mod b means the remainder after dividing a by b.
    ///
    /// This calculation is done using the shift-register method of multiplying and
    /// taking the remainder.  The register is initialized to zero, and for each
    /// incoming bit, x^32 is added mod p to the register if the bit is a one (where
    /// x^32 mod p is p+x^32 = x^26+...+1), and the register is multiplied mod p by
    /// x (which is shifting right by one and adding x^32 mod p if the bit shifted
    /// out is a one).  We start with the highest power (least significant bit) of
    /// q and repeat for all eight bits of q.
    ///
    /// The table is simply the CRC of all possible eight bit values.  This is all
    /// the information needed to generate CRC's on data a byte at a time for all
    /// combinations of CRC register values and incoming bytes.
    /// </remarks>
    public sealed class Crc32Helper : IChecksum
    {
        #region Instance Fields
        readonly static uint crcInit = 0xFFFFFFFF;
        readonly static uint crcXor = 0xFFFFFFFF;

        readonly static uint[] crcTable = {
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,

            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,

            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,

            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,

            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,

            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,

            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000, 0x00000000, 0x00000000, 0x00000000, 0x00000000,
            0x00000000
        };

        /// <summary>
        /// The CRC data checksum so far.
        /// </summary>
        uint checkValue;
        #endregion

        internal static uint ComputeCrc32(uint oldCrc, byte bval)
        {
            return (uint)(Crc32Helper.crcTable[(oldCrc ^ bval) & 0xFF] ^ (oldCrc >> 8));
        }

        /// <summary>
        /// Initialise a default instance of <see cref="Crc32"></see>
        /// </summary>
        public Crc32Helper()
        {
            Reset();
        }

        /// <summary>
        /// Resets the CRC data checksum as if no update was ever called.
        /// </summary>
        public void Reset()
        {
            checkValue = crcInit;
        }

        /// <summary>
        /// Returns the CRC data checksum computed so far.
        /// </summary>
        /// <remarks>Reversed Out = false</remarks>
        public long Value
        {
            get
            {
                return (long)(checkValue ^ crcXor);
            }
        }

        /// <summary>
        /// Updates the checksum with the int bval.
        /// </summary>
        /// <param name = "bval">
        /// the byte is taken as the lower 8 bits of bval
        /// </param>
        /// <remarks>Reversed Data = true</remarks>
        public void Update(int bval)
        {
            checkValue = unchecked(crcTable[(checkValue ^ bval) & 0xFF] ^ (checkValue >> 8));
        }

        /// <summary>
        /// Updates the CRC data checksum with the bytes taken from 
        /// a block of data.
        /// </summary>
        /// <param name="buffer">Contains the data to update the CRC with.</param>
        public void Update(byte[] buffer)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            Update(new ArraySegment<byte>(buffer, 0, buffer.Length));
        }

        /// <summary>
        /// Update CRC data checksum based on a portion of a block of data
        /// </summary>
        /// <param name = "segment">
        /// The chunk of data to add
        /// </param>
        public void Update(ArraySegment<byte> segment)
        {
            for (int i = 0; i < segment.Count; i++)
            {
                Update(segment.Array[i]);
            }
        }
    }
}

