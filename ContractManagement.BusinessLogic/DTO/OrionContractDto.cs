namespace ContractManagement.BusinessLogic.DTO
{
    public class OrionContractDto
    {
        public string Id { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public string PolicyNumber { get; set; }
        public string ClientId { get; set; }
        public string QuotationId { get; set; }
        public string ActiveQuotationId { get; set; }
        public DateTime ContractDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime EffectDate { get; set; }
        public string IntermediaryId { get; set; }
    }
}
