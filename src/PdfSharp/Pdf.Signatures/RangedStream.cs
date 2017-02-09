using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PdfSharp.Pdf.Signatures
{
    public class RangedStream : Stream
    {
        private Range[] ranges;

        public class Range
        {

            public Range(long offset, long length)
            {
                this.Offset = offset;
                this.Length = length;
            }
            public long Offset { get; set; }
            public long Length { get; set; }
        }

        private Stream stream { get; set; }
        

        public RangedStream(Stream originalStrem, List<Range> ranges)
        {
            this.stream = originalStrem;            

            long previousPosition = 0;           

            this.ranges = ranges.OrderBy(item => item.Offset).ToArray();
            foreach (var range in ranges)
            {
                if (range.Offset < previousPosition)
                    throw new Exception("Ranges are not continuous");
                previousPosition = range.Offset + range.Length ;
            }
        }


        public override bool CanRead
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool CanSeek
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return ranges.Sum(item => item.Length);
            }
        }


        private IEnumerable<Range> GetPreviousRanges(long position)
        {
            return ranges.Where(item => item.Offset < position && item.Offset + item.Length < position);
        }

        private Range GetCurrentRange(long position)
        {
             return ranges.FirstOrDefault(item => item.Offset <= position && item.Offset + item.Length > position); 
        }
        
       

        public override long Position
        {
            get
            {
                return GetPreviousRanges(stream.Position).Sum(item => item.Length) + stream.Position - GetCurrentRange(stream.Position).Offset;
            }

            set
            {
                Range currentRange = null;
                List<Range> previousRanges = new List<Range>();
                long maxPosition = 0;
                foreach (var range in ranges)
                {
                    currentRange = range;
                    maxPosition += range.Length;
                    if (maxPosition > value)
                        break;
                    previousRanges.Add(range);
                }

                long positionInCurrentRange = value - previousRanges.Sum(item => item.Length);
                stream.Position = currentRange.Offset + positionInCurrentRange;               
            }
        }

        

        public override void Flush()
        {
            throw new NotImplementedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            
            var length = stream.Length;
            int retVal = 0;
            for (int i = 0; i < count; i++)
            {

                if (stream.Position == length)
                {
                    break;
                }

                PerformSkipIfNeeded();
                retVal += stream.Read(buffer, offset++, 1);
               
            }
          
            return retVal;
        }
               

        private void PerformSkipIfNeeded()
        {
            var currentRange = GetCurrentRange(stream.Position);
            
            if (currentRange == null)
                stream.Position = GetNextRange().Offset;
        }

        private Range GetNextRange()
        {
            return ranges.OrderBy(item => item.Offset).First(item => item.Offset > stream.Position);
        }


        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            throw new NotImplementedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }
    }
}
