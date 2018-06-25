namespace CamlBuilder
{
    /// <summary>
    /// Specifies types of reference for a field in a list.
    /// </summary>
    public enum ValueType
    {
        /// <summary>
        /// Indicates a Text field type
        /// </summary>
        Text,

        /// <summary>
        /// Indicates a DateTime field type
        /// </summary>
        DateTime,

        /// <summary>
        /// Indicates a Integer field type
        /// </summary>
        Integer,

        /// <summary>
        /// Indicates a Note field type
        /// </summary>
        Note,

        /// <summary>
        /// Indicates a Choice field type
        /// </summary>
        Choice,

        /// <summary>
        /// Indicates a Number field type
        /// </summary>
        Number,

        /// <summary>
        /// Indicates a Guid field type
        /// </summary>
        Guid,

        /// <summary>
        /// Indicates a Boolean field type
        /// </summary>
        Boolean,

        /// <summary>
        /// Indicates a Counter field type
        /// </summary>
        Counter,

        /// <summary>
        /// Indicates a Currency field type
        /// </summary>
        Currency,

        /// <summary>
        /// Indicates an URL field type
        /// </summary>
        Url,
        /// <summary>
        /// Indicates a Computed field type
        /// </summary>
        Computed,

        /// <summary>
        /// Indicates a Lookup field type
        /// </summary>
        Lookup,

        /// <summary>
        /// Indicates a File field type
        /// </summary>
        File,

        /// <summary>
        /// Indicates an User field type
        /// </summary>
        User,

        /// <summary>
        /// Indicates an Attachments field type
        /// </summary>
        Attachments,

        /// <summary>
        /// Indicates a MultiChoice field type
        /// </summary>
        MultiChoice,

        /// <summary>
        /// Indicates a GridChoice field type
        /// </summary>
        GridChoice,

        /// <summary>
        /// Indicates a Threading field type
        /// </summary>
        Threading,

        /// <summary>
        /// Indicates a CrossProjectLink field type
        /// </summary>
        CrossProjectLink,

        /// <summary>
        /// Indicates a Recurrence field type
        /// </summary>
        Recurrence,

        /// <summary>
        /// Indicates a ModStat field type
        /// </summary>
        ModStat,

        /// <summary>
        /// Indicates a ContentTypeId field type
        /// </summary>
        ContentTypeId,

        /// <summary>
        /// Indicates a WorkflowStatus field type
        /// </summary>
        WorkflowStatus,

        /// <summary>
        /// Indicates a AllDayEvent field type
        /// </summary>
        AllDayEvent,

        /// <summary>
        /// Indicates an Error field type
        /// </summary>
        Error,

        /// <summary>
        /// Indicates a WorkflowEventType field type
        /// </summary>
        WorkflowEventType
    }
}
