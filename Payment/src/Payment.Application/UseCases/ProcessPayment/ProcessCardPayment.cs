﻿namespace Payment.Application.UseCases
{
    using Payment.Application.Port;
    using Payment.Domain;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Process Payment Use Case
    /// </summary>
    public class ProcessCardPayment : IUseCase<ProcessPaymentInput>
    {
        private readonly IPaymentWriteRepository _paymentRepository;
        private readonly IProcessPaymentOutputPort _paymentOutputPort;
        private readonly IBankService _bankService;

        public ProcessCardPayment(IPaymentWriteRepository paymentRepository, IProcessPaymentOutputPort paymentOutputPort, IBankService bankService)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _bankService = bankService ?? throw new ArgumentNullException(nameof(bankService));
            _paymentOutputPort = paymentOutputPort ?? throw new ArgumentNullException(nameof(paymentOutputPort));
        }

        public async Task Execute(ProcessPaymentInput input)
        {
            if (input is null)
            {
                _paymentOutputPort.BadRequest("input is null");
                return;
            }


            var payment = Payment.CreateNewCardPayment(input.Card, input.Amount, input.BeneficiaryAlias);

            await _paymentRepository.AddPaymentAsync(payment).ConfigureAwait(false);

            var bankResult = await _bankService.SubmitCardPaymentAsync(payment).ConfigureAwait(false);

            await _paymentRepository.UpdatePaymentStatusAsync(bankResult.PaymentId, bankResult.PaymentStatus).ConfigureAwait(false);

            _paymentOutputPort.OK(BuildOutput(bankResult));
        }


        private ProcessPaymentOutput BuildOutput(BankResult bankResult)
        {
            return new ProcessPaymentOutput()
            {
                PaymentId = bankResult.PaymentId,
                PaymentStatus = bankResult.PaymentStatus
            };
        }
    }
}
