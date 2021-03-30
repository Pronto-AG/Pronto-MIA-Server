namespace Pronto_MIA.Domain.Entities
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Class which represents a Speaker.
    /// </summary>
    public class Speaker
    {
        /// <summary>
        /// Gets or sets unique Id of the speaker.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the speaker.
        /// </summary>
        [Required]
        [StringLength(200)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the bio of the speaker.
        /// </summary>
        [StringLength(4000)]
        public string Bio { get; set; }

        /// <summary>
        /// Gets or sets the website of the speaker.
        /// </summary>
        [StringLength(1000)]
        public virtual string WebSite { get; set; }
    }
}
