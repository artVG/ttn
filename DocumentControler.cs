using System;
using System.Collections.Generic;
using System.Linq;

namespace ttn
{
    /// <summary>
    /// class for work with Document instances in database
    /// </summary>
    class DocumentController
    {
        private DocumentContext Context; //connection to database

        /// <summary>
        /// connect to specified database
        /// </summary>
        /// <param name="name">database path</param>
        public DocumentController(string name)
        {
            Context = new DocumentContext(name);
        }

        /// <summary>
        /// add document range to database
        /// </summary>
        /// <param name="series">series of documents in range</param>
        /// <param name="from">first document number</param>
        /// <param name="to">last document number</param>
        /// <param name="addDate">date of operation</param>
        public void AddRange(string series, int from, int to, DateTime addDate)
        {
            for (int i = from; i < to + 1; i++)
            {
                Context.Add(new Document()
                {
                    Series = series,
                    Number = i,
                    InDate = addDate
                });
            }
            try
            {
                Context.SaveChanges();
            }
            //process exception if at least one document in given Reagan already exists in database
            catch (Microsoft.EntityFrameworkCore.DbUpdateException ex)
            {
                if (ex.ToString().ToLower().Contains("unique constraint failed"))
                {
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException("Document already exists");
                }
                else
                {
                    throw ex;
                }
            }
        }

        /// <summary>
        /// remove given range from database
        /// </summary>
        /// <param name="series">series of documents in range</param>
        /// <param name="from">first document number</param>
        /// <param name="to">last document number</param>
        public void DeleteRange(string series, int from, int to)
        {
            for (int i = from; i < to + 1; i++)
            {
                //create document with DBkey values and delete
                Document d = new Document() { Series = series, Number = i };
                Context.Remove(d);
            }
            //save changes to database
            Context.SaveChanges();
        }

        /// <summary>
        /// mark document range as spoiled if it exists in database and was marked as used
        /// </summary>
        /// <param name="series">series of documents in range</param>
        /// <param name="from">first document number</param>
        /// <param name="to">last document number</param>
        public void SpoiledRange(string series, int from, int to)
        {
            for (int i = from; i < to + 1; i++)
            {
                //search in database for document with specified key values
                Document d = Context.Documents.Find(series, i);
                if (d == null)
                {
                    //throw exception if document does not exist
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException(String.Format("Document does not exist {0} {1}", series, i));
                }
                if (d.OutDate == default)
                {
                    //throw exception if document was not used
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException(String.Format("Document {0} must be used", d));
                }
                //mark document as spoiled
                d.Spoiled = true;
                //update document state in database
                Context.Documents.Update(d);
            }
            //save changes to database
            Context.SaveChanges();
        }

        /// <summary>
        /// delete spoiled mark from documents
        /// </summary>
        /// <param name="series">series of documents in range</param>
        /// <param name="from">first document number</param>
        /// <param name="to">last document number</param>
        public void UnspoiledRange(string series, int from, int to)
        {
            for (int i = from; i < to + 1; i++)
            {
                //search in database for document with specified key values
                Document d = Context.Documents.Find(series, i);
                if (d == null)
                {
                    //throw exception if document does not exist
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException(String.Format("Document does not exist {0} {1}", series, i));
                }
                //mark document as not spoiled
                d.Spoiled = false;
                //update document state in database
                Context.Documents.Update(d);
            }
            //save changes to database
            Context.SaveChanges();
        }

        /// <summary>
        /// mark document range as used by adding 'used date'
        /// </summary>
        /// <param name="series">series of documents in range</param>
        /// <param name="from">first document number</param>
        /// <param name="to">last document number</param>
        /// <param name="outDate">date the document has been used</param>
        public void UsedRange(string series, int from, int to, DateTime outDate)
        {
            for (int i = from; i < to + 1; i++)
            {
                //search in database for document with specified key values
                Document d = Context.Documents.Find(series, i);
                if (d == null)
                {
                    //throw exception if document does not exist
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException(String.Format("Document does not exist {0} {1}", series, i));
                }
                if (d.OutDate != default)
                {
                    //throw exception if document was already used
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException(String.Format("Document {0} already used", d));
                }
                if (outDate < d.InDate)
                {
                    //throw exception on attempt to set used date before the date document has been added to database
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException(String.Format("Document's used date must be equal or more than InDate {0}", d));
                }
                //set document's date when it was used
                d.OutDate = outDate;
                //update document state in database
                Context.Documents.Update(d);
            }
            //save changes to database
            Context.SaveChanges();
        }


        /// <summary>
        /// delete used date from documents. ALERT!!!: Spoiled mark will be deleted also
        /// </summary>
        /// <param name="series">series of documents in range</param>
        /// <param name="from">first document number</param>
        /// <param name="to">last document number</param>
        public void UnusedRange(string series, int from, int to)
        {
            for (int i = from; i < to + 1; i++)
            {
                //search in database for document with specified key values
                Document d = Context.Documents.Find(series, i);
                if (d == null)
                {
                    //throw exception if document does not exist
                    throw new Microsoft.EntityFrameworkCore.DbUpdateException(String.Format("Document does not exist {0} {1}", series, i));
                }
                //delete document's date when it was used
                d.OutDate = default;
                //mark document as not spoiled
                d.Spoiled = false;
                //update document state in database
                Context.Documents.Update(d);
            }
            //save changes to database
            Context.SaveChanges();
        }

        /// <summary>
        /// get all unused, unspoiled documents at given date sorted by series and number
        /// </summary>
        /// <param name="date">date of report (included)</param>
        /// <returns>list of Document ranges [first doc, last doc]</returns>
        public List<Document[]> GetLeftAt(DateTime date)
        {
            IQueryable<Document> documents = Context.Documents
                .Where(d => (d.InDate <= date) && (d.OutDate >= date || d.OutDate == default))
                .OrderBy(d => d.Series)
                .OrderBy(d => d.Number);
            return ToRanges(documents);
        }

        /// <summary>
        /// gel all used (including spoiled) documents in specified dates range sorted by series and number
        /// </summary>
        /// <param name="from">begin date (not included)</param>
        /// <param name="to">end date (included)</param>
        /// <returns>list of Document ranges [first doc, last doc]</returns>
        public List<Document[]> GetUsedIn(DateTime from, DateTime to)
        {
            IQueryable<Document> documents = Context.Documents
                .Where(d => d.OutDate > from && d.OutDate <= to)
                .OrderBy(d => d.Series)
                .OrderBy(d => d.Number);
            return ToRanges(documents);
        }

        /// <summary>
        /// gel all added (including used and spoiled) documents in specified dates range sorted by series and number
        /// </summary>
        /// <param name="from">begin date (included)</param>
        /// <param name="to">end date (included)</param>
        /// <returns>list of Document ranges [first doc, last doc]</returns>
        public List<Document[]> GetAddedIn(DateTime from, DateTime to)
        {
            IQueryable<Document> documents = Context.Documents
                .Where(d => d.InDate >= from && d.InDate <= to)
                .OrderBy(d => d.Series)
                .OrderBy(d => d.Number);
            return ToRanges(documents);
        }

        /// <summary>
        ///  gel all used but not spoiled documents in specified dates range sorted by series and number
        /// </summary>
        /// <param name="from">begin date (included)</param>
        /// <param name="to">end date (included)</param>
        /// <returns>list of Document ranges [first doc, last doc]</returns>
        public List<Document[]> GetUsedInWitoutSpoiled(DateTime from, DateTime to)
        {
            IQueryable<Document> documents = Context.Documents
                .Where(d => d.OutDate >= from && d.OutDate <= to && d.Spoiled == false)
                .OrderBy(d => d.Series)
                .OrderBy(d => d.Number);
            return ToRanges(documents);
        }

        /// <summary>
        /// gel all spoiled documents in specified dates range sorted by series and number
        /// </summary>
        /// <param name="from">begin date (included)</param>
        /// <param name="to">end date (included)</param>
        /// <returns>list of Document ranges [first doc, last doc]</returns>
        public List<Document[]> GetSpoiledIn(DateTime from, DateTime to)
        {
            IQueryable<Document> documents = Context.Documents
                .Where(d => d.OutDate >= from && d.OutDate <= to && d.Spoiled == true)
                .OrderBy(d => d.Series)
                .OrderBy(d => d.Number);
            return ToRanges(documents);
        }

        /// <summary>
        /// transforms list  of Documents into list of Document ranges
        /// </summary>
        /// <param name="documents">sorted from smallest to largest list of Document instances</param>
        /// <returns>list of Document ranges [first doc, last doc]</returns>
        private List<Document[]> ToRanges(IQueryable<Document> documents)
        {
            //-----at first find border values-----
            //previous Document in documents
            Document previous = default;
            //list of Document first and last and border values, when series changes in next Document
            //or next doc number it more then previous+1 
            //ATTENTION  borderValues length MUST BE 0 OR EVEN
            List<Document> borderValues = new List<Document>();
            //if documents list is empty
            if (documents.Count() < 1)
            {
                //return empty list
                return new List<Document[]>();
            }
            //if documents list has one or more items
            foreach (Document d in documents)
            {
                //if first in list of documents param
                if (previous == default)
                {
                    previous = d;
                    borderValues.Add(d);
                }
                //if next in list of documents param not next to previous (different series
                //or next doc number it more then previous+1) add border value to list
                else if (!(d.NextTo(previous)))
                {
                    borderValues.Add(previous);
                    borderValues.Add(d);
                }
                previous = d;
            }
            //if last in list of documents param
            borderValues.Add(documents.Last());
            //end-----at first find border values-----

            //------transform border values to ranges [1,2]
            List<Document[]> ranges = new List<Document[]>();
            //first in range [1,2]
            Document first = default;
            foreach (Document val in borderValues)
            {
                //first item in border values hallways will be first in range
                if (first == default)
                {
                    first = val;
                }
                //if first was gotten then this is the second
                else
                {
                    ranges.Add(new Document[] { first, val });
                    //set to default because next is first in range
                    first = default;
                }
            }
            return ranges;
        }

    }
}
