namespace CamlBuilder
{
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Class which represents a CAML query.
    /// </summary>
    /// <summary>
    /// Defines a CAML query. This class has no constructors available. To instanciate a
    /// new query use public static methods.
    /// </summary>
    public class Query
    {
        private readonly List<FieldReference> orderByFields = new List<FieldReference>();

        private readonly List<FieldReference> groupByFields = new List<FieldReference>();

        /// <summary>
        /// Gets the statement holded by this query.
        /// </summary>
        public Statement Statement { get; }

        private Query(Statement statement)
        {
            Statement = statement;
        }

        /// <summary>
        /// Instanciates a new <i>Query</i> with the specified inner <paramref name="statement"/>
        /// </summary>
        /// <param name="statement"></param>
        /// <returns></returns>
        public static Query Build(Statement statement)
        {
            return new Query(statement);
        }

        /// <summary>
        /// Returns query's CAML string representation
        /// </summary>
        /// <param name="whereClauseOnly">true to return only query's Where; otherwise false</param>
        /// <returns>Query CAML string surrounded by Query element or only the Where clause</returns>
        public string GetCaml(bool whereClauseOnly)
        {
            if (whereClauseOnly)
            {
                return GetWhereCaml();
            }

            return $@"<Query>{GetWhereCaml()}{GetGroupByCaml()}{GetOrderByCaml()}</Query>";
        }

        /// <summary>
        /// Returns query's CAML string representation surrounded by Query element
        /// </summary>
        /// <returns>Query CAML string surrounded by Query element.</returns>
        public string GetCaml()
        {
            return GetCaml(false);
        }

        /// <summary>
        /// Adds a new query sort order relatively to a specified <paramref name="fieldRef"/>.
        /// </summary>
        /// <param name="fieldRef">Reference to the field where to perform the ordering on.</param>
        /// <returns>Returns the query itself.</returns>
        /// <remarks>Use <see cref="FieldReference.Ascending"/> with false value to specify descending order.</remarks>
        public Query OrderBy(FieldReference fieldRef)
        {
            orderByFields.Add(fieldRef);
            return this;
        }

        /// <summary>
        /// Adds a collection of sort orders relatively to specified <paramref name="fieldRefs"/>.
        /// </summary>
        /// <param name="fieldRefs">References to the fields where to perform the ordering on.</param>
        /// <returns>Returns the query itself.</returns>
        /// <remarks>Use <see cref="FieldReference.Ascending"/> with false value to specify descending order.</remarks>
        public Query OrderBy(IEnumerable<FieldReference> fieldRefs)
        {
            orderByFields.AddRange(fieldRefs);
            return this;
        }

        /// <summary>
        /// Specify the query's group-by options. Query will be grouped by specified <paramref name="fieldRef"/>.
        /// </summary>
        /// <param name="fieldRef">Reference to the field to group by.</param>
        /// <returns>Returns the query itself.</returns>
        public Query GroupBy(FieldReference fieldRef)
        {
            groupByFields.Add(fieldRef);
            return this;
        }

        /// <summary>
        /// Specify the query's group-by options. Query will be grouped by specified <paramref name="fieldRefs"/>.
        /// </summary>
        /// <param name="fieldRefs">References to the fields to group by.</param>
        /// <returns>Returns the query itself.</returns>
        public Query GroupBy(IEnumerable<FieldReference> fieldRefs)
        {
            groupByFields.AddRange(fieldRefs);
            return this;
        }

        private string GetWhereCaml()
        {
            if (Statement == null)
            {
                return string.Empty;
            }

            return $@"<Where>{Statement.GetCaml()}</Where>";
        }

        private string GetOrderByCaml()
        {
            if (orderByFields.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            orderByFields.ForEach(o => sb.AppendLine(o.GetCaml()));

            return $@"<OrderBy>{sb}</OrderBy>";
        }

        private string GetGroupByCaml()
        {
            if (groupByFields.Count == 0)
            {
                return string.Empty;
            }

            var sb = new StringBuilder();
            groupByFields.ForEach(g => sb.AppendLine(g.GetCaml()));

            return $@"<GroupBy>{sb}</GroupBy>";
        }
    }
}
