using System;
using System.Collections.Generic;
using System.Text;

namespace ttn
{
    /// <summary>
    /// single document data
    /// </summary>
    class Document
    {
        public string Series { get; set; } //document series
        public int Number { get; set; } //document number
        public DateTime InDate { get; set; } //ingoing document date
        public DateTime OutDate { get; set; } = default; //date when document has been used
        public bool Spoiled { get; set; } = false; //document spoiled mark

        /// <summary>
        /// check if this document goes after the other
        /// </summary>
        /// <param name="other">another Document</param>
        /// <returns>true if this and other Documents have the same series
        /// and this Documents's number is n+1 to he other</returns>
        public bool NextTo(Document other)
        {
            return this.SameSeries(other) &&
                this.Number == other.Number + 1;
        }

        /// <summary>
        /// check if this and other Documents have the same series
        /// </summary>
        /// <param name="other">another Document</param>
        /// <returns>true if this and other Documents have the same series</returns>
        public bool SameSeries(Document other)
        {
            return this.Series == other.Series;
        }

        /// <summary>
        /// count number of documents in range from this to other
        /// </summary>
        /// <param name="other">another Document</param>
        /// <returns>number of documents in range from this to other.
        /// 0 if Documents have different Series</returns>
        public int CountAmountTo(Document other)
        {
            if (!this.SameSeries(other))
            {
                return 0;
            }
            else
            {
                return other.Number - this.Number + 1;
            }
        }

        public override bool Equals(object other)
        {
            if (GetType() == other.GetType())
            {
                Document otherTmp = (Document)other;
                return SameSeries(otherTmp) &&
                    Number == otherTmp.Number &&
                    InDate == otherTmp.InDate &&
                    OutDate == otherTmp.OutDate &&
                    Spoiled == otherTmp.Spoiled;
            }
            else
            {
                return false;
            }
        }

        public override string ToString()
        {
            return String.Format("{0} {1} {2} {3} {4}", Series, Number, InDate, OutDate, Spoiled);
        }
    }
}
