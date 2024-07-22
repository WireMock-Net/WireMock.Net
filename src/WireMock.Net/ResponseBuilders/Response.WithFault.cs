// Copyright © WireMock.Net

namespace WireMock.ResponseBuilders;

public partial class Response
{
    /// <inheritdoc cref="IFaultResponseBuilder.WithFault(FaultType, double?)"/>
    public IResponseBuilder WithFault(FaultType faultType, double? percentage = null)
    {
        ResponseMessage.FaultType = faultType;
        ResponseMessage.FaultPercentage = percentage;

        return this;
    }
}