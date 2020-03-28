﻿using System;

namespace PCSC.Iso7816
{
    /// <summary>A class the can be used to build or parse the CLA (Class byte) of a APDU.</summary>
    public class ClassByte
    {
        private const byte SECURE_MESSAGING_MASK = 0x4 + 0x8;
        private const byte LOGICAL_CHANNEL_NUMBER_MASK = 0x1 + 0x2;

        private byte _cla;

        /// <summary>Initializes a new instance of the <see cref="ClassByte" /> class.</summary>
        /// <param name="cla">The CLA as byte that will be parsed.</param>
        public ClassByte(byte cla) {
            this._cla = cla;
        }

        /// <summary>Initializes a new instance of the <see cref="ClassByte" /> class.</summary>
        /// <param name="highPart">The high part of the CLA</param>
        /// <param name="secureMessagingFormat">The secure messaging format.</param>
        /// <param name="logicalChannelNumber">The logical channel number.</param>
        /// <exception cref="System.ArgumentOutOfRangeException">logicalChannelNumber;Logical channels must be in the range between 0 and 3.</exception>
        public ClassByte(ClaHighPart highPart, SecureMessagingFormat secureMessagingFormat, int logicalChannelNumber) {
            if (logicalChannelNumber > 3 || logicalChannelNumber < 0) {
                throw new ArgumentOutOfRangeException(
                    "logicalChannelNumber",
                    "Logical channels must be in the range between 0 and 3.");
            }
            this._cla = (byte)((int)highPart | (int)secureMessagingFormat | logicalChannelNumber);
        }

        /// <summary>Returns the CLA as byte.</summary>
        public byte Value {
            get { return this._cla; }
            set { this._cla = value; }
        }

        /// <summary>Gets or sets the high part of the CLA </summary>
        /// <value>The high part of the CLA.</value>
        public ClaHighPart HighPart {
            get {
                // get the high part (b8,b7,b6,b5)
                var h = (byte)(0xF0 & this._cla);

                // return the high part
                return (ClaHighPart) h;
            }
            set {
                // save the low part (b4,b3,b2,b1)
                var l = (byte)(0x0F & this._cla);

                // combine the user specified high part with the saved low part
                this._cla = (byte)((int)value | l);
            }
        }

        /// <summary>Gets or sets the secure messaging (SM) format. </summary>
        public SecureMessagingFormat Security {
            get {
                var sec = (byte)(this._cla & SECURE_MESSAGING_MASK);
                return (SecureMessagingFormat) sec;
            }
            set {
                byte inversemask;
                unchecked {
                    inversemask = (byte) (~(SECURE_MESSAGING_MASK));
                }
                // save old settings
                var tmp = (byte)(this._cla & inversemask);

                // set new Secure Messaging Format
                this._cla = (byte)(tmp | (int)value);
            }
        }

        /// <summary>Gets or sets the logical channel number.</summary>
        /// <value>The logical channel number.</value>
        /// <exception cref="System.ArgumentOutOfRangeException">value;Logical channels must be in the range between 0 and 3.</exception>
        public int LogicalChannel {
            get {
                var logch = (this._cla & LOGICAL_CHANNEL_NUMBER_MASK);
                return logch;
            }
            set {
                if (value > 3 || value < 0) {
                    throw new ArgumentOutOfRangeException("value",
                        "Logical channels must be in the range between 0 and 3.");
                }

                byte inversemask;
                unchecked {
                    inversemask = (byte) (~(LOGICAL_CHANNEL_NUMBER_MASK));
                }
                // save old settings
                var tmp = (byte)(this._cla & inversemask);

                // set new logical channel setting
                this._cla = (byte)(tmp | value);
            }
        }

        /// <summary>Implicitly converts a <see cref="ClassByte"/> to a <see cref="byte"/>.</summary>
        /// <param name="classByte"></param>
        /// <returns>The CLA as byte</returns>
        public static implicit operator byte(ClassByte classByte) {
            return classByte._cla;
        }

        /// <summary>Implicitly converts a <see cref="byte"/> to an <see cref="ClassByte"/> instance.</summary>
        /// <param name="byte">CLA as byte.</param>
        /// <returns>The parsed CLA.</returns>
        public static implicit operator ClassByte(byte @byte) {
            return new ClassByte(@byte);
        }
    }
}