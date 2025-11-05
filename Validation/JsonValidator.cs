namespace AppKreator.Validation
{
    using Json.Schema;

    public static class JsonValidator
    {
        private static readonly JsonSchema _schema;
        static JsonValidator()
        {
            var schemaText = File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "Validation", "GenPayloadSchema.json"));
            _schema = JsonSchema.FromText(schemaText);
        }

        public static (bool ok, string? error) Validate(string json)
        {
            try
            {
                var node = System.Text.Json.Nodes.JsonNode.Parse(json)!;
                var result = _schema.Evaluate(node);
                if (result.IsValid)
                    return (true, null);

                // Collect errors from the result and all its details recursively
                var errors = new List<string>();
                void CollectErrors(EvaluationResults res)
                {
                    if (res.Errors != null && res.Errors.Count > 0)
                        errors.AddRange(res.Errors.Values);
                    if (res.HasDetails && res.Details != null)
                    {
                        foreach (var detail in res.Details)
                            CollectErrors(detail);
                    }
                }
                CollectErrors(result);

                return (false, string.Join("; ", errors));
            }
            catch (Exception ex)
            {
                return (false, ex.Message);
            }
        }
    }

}
