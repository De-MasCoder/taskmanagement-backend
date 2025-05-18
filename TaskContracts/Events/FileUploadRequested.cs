using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskContracts.Events
{
    public class FileUploadRequested
    {
        public Guid TaskId { get; set; }
        public required string FileName { get; set; }
        public required byte[] FileBytes { get; set; }
        public required string ContentType { get; set; }
    }

}
