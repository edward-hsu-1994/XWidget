using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace XWidget.EFLogic.Test.Models {
    [ModelMetadataType(typeof(NoteMetadata))]
    public class Note {
        public Note() {
        }
        [Key]
        public virtual Guid Id { get; set; }

        public virtual Guid CategoryId { get; set; }
        public virtual string Title { get; set; }
        public virtual string Context { get; set; }

        public virtual Guid? UserDataId { get; set; }
        [ForeignKey("CategoryId")]
        [InverseProperty("Notes")]
        public virtual Category Category { get; set; }

        [ForeignKey("UserDataId")]
        [InverseProperty("Note")]
        public virtual UserData UserData { get; set; }
    }

    public class NoteMetadata {
        public bool ShouldRemoveCascadeCategory() => false;
    }
}
