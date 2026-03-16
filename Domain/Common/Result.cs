using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Common
{
    public class Result
    {
        protected Result(bool isSuccess, IEnumerable<Error>? errors = null)
        {
            IsSuccess = isSuccess;
            Errors = errors?.ToList() ?? new List<Error>();
        }

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public IReadOnlyCollection<Error> Errors { get; }

        public static Result Success() => new(true);
        public static Result Failure(params Error[] errors) => new(false, errors);

        // Combinar múltiples resultados en uno solo
        public static Result Combine(params Result[] results)
        {
            var errors = results
                .Where(r => r.IsFailure)
                .SelectMany(r => r.Errors)
                .ToList();

            return errors.Any()
                ? Failure(errors.ToArray())
                : Success();
        }
    }

    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        private Result(TValue? value, bool isSuccess, IEnumerable<Error>? errors = null)
            : base(isSuccess, errors) => _value = value;

        public TValue Value => IsSuccess
            ? _value!
            : throw new InvalidOperationException("El resultado fallido no tiene valor.");

        public static Result<TValue> Success(TValue value) => new(value, true);
        public static new Result<TValue> Failure(params Error[] errors) => new(default, false, errors);
    }


}
