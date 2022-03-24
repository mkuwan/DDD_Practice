using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;


namespace SampleConsole.Application.Behaviors
{
    /// <summary>
    /// MediatRのIPipelineBehaviorを継承することで
    /// MediatRのSentメソッドが実行された時に、IRequestHandlerよりも先に、
    /// IPipelineBehaviorを継承をしたこちらのメソッドが呼ばれます。
    /// 　正確には、このパイプラインで示された順序でメソッドが実行されるということで
    /// 　このIPipelineBehaviorを継承したクラスが最初に呼ばれ、そこでメソッドの順序が決定されます
    /// 　そして、このクラスで参照しているTRequestがCreateScheduleCommandの場合、
    /// 　それを継承しているCreateScheduleCommandValidator : AbstractValidator<CreateScheduleCommand>が先に呼ばれ、
    /// 　ルールが適応されます。
    /// 　このルールに基づいて、以下のValidationが実行されてエラーがあるかチェックされます。
    /// 　1.Controller => Send(IRequest)
    ///   2.Validator
    ///   3.Behavior
    ///   4.IRequestHandler
    ///
    /// 　つまり、IRequest, IRequestHandlerを継承したクラスごとに、AbstractValidatorを継承したValidatorを作成して
    /// 　検証することができます
    /// 
    /// 　next()メソッドで、次にあるべきものが呼ばれます
    /// 　ここでは、IRequestHandlerが呼ばれるということになります
    /// </summary>
    /// <typeparam name="TRequest"></typeparam>
    /// <typeparam name="TResponse"></typeparam>
    public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> 
        where TRequest : IRequest<TResponse>    
    {

        private readonly IEnumerable<IValidator<TRequest>> _validators;

        public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
        {
            _validators = validators;
        }

        public async Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
        {
            var context = new ValidationContext<TRequest>(request);
            var failures = _validators
                .Select(x => x.Validate(context))
                .SelectMany(x => x.Errors)
                .Where(x => !x.Equals(null))
                .ToList();

            if (failures.Any())
            {
                // エラーはこれでなくても、TResponseで返すなどいろいろあります
                throw new ValidationException(failures);
                
            }

            return await next();
        }
    }
}
