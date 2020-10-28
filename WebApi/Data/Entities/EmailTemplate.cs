using System.ComponentModel.DataAnnotations;

namespace WebApi.Data.Entities
{
    public class EmailTemplate : IntBase
    {
        /// <summary>
        /// Gets or sets the template Name
        /// </summary>
        ///
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the template Body
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// Gets or sets the template Subject
        /// </summary>
        public string Subject { get; set; }
    }
}