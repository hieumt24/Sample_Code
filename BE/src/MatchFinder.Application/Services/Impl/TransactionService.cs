using AutoMapper;
using MatchFinder.Application.Constants;
using MatchFinder.Application.Models.Requests;
using MatchFinder.Application.Models.Responses;
using MatchFinder.Domain.Exceptions;
using MatchFinder.Domain.Interfaces;
using MatchFinder.Domain.Models;
using MatchFinder.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Net.payOS.Types;

namespace MatchFinder.Application.Services.Impl
{
    public class TransactionService : ITransactionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        private readonly IPayOSPaymentService _payOSPaymentService;
        private readonly INotificationService _notificationService;

        public TransactionService(IUnitOfWork unitOfWork, IMapper mapper, IPayOSPaymentService payOSPaymentService, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _payOSPaymentService = payOSPaymentService;
            _notificationService = notificationService;
        }

        public async Task<decimal> GetAmountAsync(int uid)
        {
            if (await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid) == null)
                throw new ConflictException("User not exist!");

            decimal userAmountProactive = await _unitOfWork.TransactionRepository
                            .GetQueryable(t => (!EF.Functions.Like(t.Type, TransactionType.DEBTPAYMENT)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.PENDING)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.CANCELLED))
                                              && t.UserId == uid)
                            .SumAsync(t => t.Amount);

            decimal userAmountPassive = await _unitOfWork.TransactionRepository
                            .GetQueryable(t => (!EF.Functions.Like(t.Type, TransactionType.DEBTPAYMENT)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.PENDING)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.CANCELLED))
                                              && t.ReciverId == uid)
                            .SumAsync(t => t.Amount);

            decimal userAmountDebtPayed = await _unitOfWork.TransactionRepository
                            .GetQueryable(t => EF.Functions.Like(t.Type, TransactionType.DEBTPAYMENT)
                                                    && EF.Functions.Like(t.Status, TransactionStatus.SUCCESSFUL)
                                              && t.ReciverId == uid)
                            .SumAsync(t => t.Amount);

            decimal result = -Math.Abs(userAmountProactive) + Math.Abs(userAmountPassive) - Math.Abs(userAmountDebtPayed);
            return result;
        }

        public async Task<decimal> GetAvailableBalanceAsync(int uid)
        {
            if (await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid) == null)
                throw new ConflictException("User not exist!");

            decimal userAmountProactive = await _unitOfWork.TransactionRepository
                            .GetQueryable(t => (!EF.Functions.Like(t.Type, TransactionType.DEBTPAYMENT)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.PENDING)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.CANCELLED))
                                              && t.UserId == uid)
                            .SumAsync(t => t.Amount);

            decimal userAmountPassive = await _unitOfWork.TransactionRepository
                            .GetQueryable(t => (!EF.Functions.Like(t.Type, TransactionType.DEBTPAYMENT)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.PENDING)
                                                && !EF.Functions.Like(t.Status, TransactionStatus.CANCELLED))
                                              && (t.Booking == null || t.Booking.Date < DateOnly.FromDateTime(DateTime.Now.AddDays(-1)))
                                              && t.ReciverId == uid)
                            .SumAsync(t => t.Amount);

            decimal userAmountDebtPayed = await _unitOfWork.TransactionRepository
                            .GetQueryable(t => EF.Functions.Like(t.Type, TransactionType.DEBTPAYMENT)
                                                    && EF.Functions.Like(t.Status, TransactionStatus.SUCCESSFUL)
                                              && t.ReciverId == uid)
                            .SumAsync(t => t.Amount);

            decimal result = -Math.Abs(userAmountProactive) + Math.Abs(userAmountPassive) - Math.Abs(userAmountDebtPayed);
            return result;
        }

        public async Task<TransactionResponse> GetByIdAsync(int id)
        {
            var trans = await _unitOfWork.TransactionRepository.GetAsync(b => b.Id == id,
                                                                        x => x.User,
                                                                        p => p.Reciver);
            if (trans == null)
                throw new NotFoundException("Transaction not exist!");

            return _mapper.Map<TransactionResponse>(trans);
        }

        public async Task<RepositoryPaginationResponse<TransactionResponse>> GetListAsync(int uid, GetTransactionsRequest request)
        {
            if (await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid) == null)
                throw new ConflictException("User not exist!");

            var trans = await _unitOfWork.TransactionRepository.GetListAsync(b => (b.UserId == uid || b.ReciverId == uid)
                                                                            && (request.FieldId == null || b.Booking.PartialField.FieldId == request.FieldId)
                                                                            && (!request.Date.HasValue || b.CreatedAt.Value.Date == request.Date) ,
                                                                            request.Limit, request.Offset,
                                                                            x => x.User,
                                                                            p => p.Reciver);

            return new RepositoryPaginationResponse<TransactionResponse>
            {
                Data = _mapper.Map<IEnumerable<TransactionResponse>>(trans.Data),
                Total = trans.Total
            };
        }

        public async Task<string> RechargeAsync(int uid, RechargeRequest request)
        {
            if (await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid) == null)
                throw new ConflictException("User not exist!");

            var trans = new MatchFinder.Domain.Entities.Transaction();
            trans.Status = TransactionStatus.PENDING;
            trans.Type = TransactionType.RECHARGE;
            trans.Amount = request.Amount;
            trans.Description = request.Description ?? "NAP TIEN VAO MATCHFINDER";
            trans.UserId = 1;
            trans.ReciverId = uid;

            await _unitOfWork.TransactionRepository.AddAsync(trans);

            if (await _unitOfWork.CommitAsync() > 0)
            {
                trans.PaymentLink = await _payOSPaymentService.createPaymentLink(trans);
                if (await _unitOfWork.CommitAsync() > 0)
                {
                    return trans.PaymentLink;
                }
                throw new ConflictException("Create payment link failed!");
            }
            throw new ConflictException("Recharge failed!");
        }

        public async Task<TransactionResponse> DebtPaymentAsync(int uid, DebtPaymentRequest request)
        {
            if (await _unitOfWork.UserRepository.GetAsync(u => u.Id == uid) == null)
                throw new ConflictException("User not exist!");

            decimal debt = await GetAvailableBalanceAsync(request.ReceiverId);

            if (debt < request.Amount)
            {
                throw new ConflictException("Maximum liabilities are " + debt);
            }

            var trans = new MatchFinder.Domain.Entities.Transaction();
            trans.Status = TransactionStatus.SUCCESSFUL;
            trans.Type = TransactionType.DEBTPAYMENT;
            trans.Amount = request.Amount;
            trans.Description = request.Description ?? "MATCHFINDER SYSTEM to REPAY DEBTS";
            trans.UserId = 1;
            trans.ReciverId = request.ReceiverId;

            await _unitOfWork.TransactionRepository.AddAsync(trans);

            if (await _unitOfWork.CommitAsync() > 0)
            {
                //var paymentLink = await _payOSPaymentService.createPaymentLink(trans);
                return _mapper.Map<TransactionResponse>(trans);
            }
            throw new ConflictException("DEBT Payment failed!");
        }

        public async Task VerifyPaymentWebhookData(WebhookType hook)
        {
            var webhookData = _payOSPaymentService.verifyPaymentWebhookData(hook);
            if (webhookData.description == "Ma giao dich thu nghiem" || webhookData.description == "VQRIO123")
                return;

            int id = unchecked((int)webhookData.orderCode);
            var trans = await _unitOfWork.TransactionRepository.GetAsync(t => t.Id == id);
            if (trans == null)
            {
                return;
            }
            if (webhookData.code == "00")
            {
                trans.Status = TransactionStatus.SUCCESSFUL;
            }
            else
            {
                trans.Status = TransactionStatus.FAILED;
            }
            _unitOfWork.TransactionRepository.Update(trans);
            await _unitOfWork.CommitAsync();
        }

        public async Task<TransactionResponse> CancelPaymentLink(int uid, int transactionId)
        {
            var trans = await _unitOfWork.TransactionRepository.GetAsync(t => t.Id == transactionId && t.ReciverId == uid, p => p.Reciver, p => p.User);

            if (trans == null)
            {
                throw new NotFoundException("Transaction not exist!");
            }
            if (trans.Status != TransactionStatus.PENDING)
            {
                throw new ConflictException("Cannot cancel payment link of this transaction! Because this transaction is " + trans.Status);
            }

            var cancelResult = await _payOSPaymentService.cancelPaymentLink(transactionId);
            if (cancelResult)
            {
                trans.Status = TransactionStatus.CANCELLED;
                _unitOfWork.TransactionRepository.Update(trans);
                if (await _unitOfWork.CommitAsync() > 0)
                {
                    return _mapper.Map<TransactionResponse>(trans);
                }
            }
            throw new ConflictException("Cancel payment link failed!");
        }
    }
}