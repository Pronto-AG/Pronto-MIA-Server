using System.Net.Http;

namespace Tests.TestDataAccess.TestManagers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using FirebaseAdmin;
    using FirebaseAdmin.Messaging;

    public class TestHelpersFirebaseMessagingManager
    {
        public static BatchResponse CreateBatchResponse(
            int count,
            int failureCount = 0)
        {
            var responses = new List<SendResponse>();
            for (var i = 0; i < failureCount; i++)
            {
                var failedResponse = CreateInstance<SendResponse>(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    new[] { typeof(FirebaseMessagingException) },
                    new object[] { CreateFirebaseException() });
                responses.Add(failedResponse);
            }

            for (var i = 0; i < (count - failureCount); i++)
            {
                var sentResponse = CreateInstance<SendResponse>(
                    BindingFlags.NonPublic | BindingFlags.Instance,
                    new[] { typeof(string) },
                    new object[] { "foo" });
                responses.Add(sentResponse);
            }

            var batchResponse = CreateInstance<BatchResponse>(
                BindingFlags.NonPublic | BindingFlags.Instance,
                new[] { typeof(IEnumerable<SendResponse>) },
                new object[] { responses });

            return batchResponse;
        }

        public static FirebaseMessagingException CreateFirebaseException()
        {
            var types = new[]
            {
                typeof(ErrorCode),
                typeof(string),
                typeof(FirebaseAdmin.Messaging.MessagingErrorCode),
                typeof(Exception),
                typeof(HttpResponseMessage),
            };
            var ctorParams = new object[]
            {
                ErrorCode.InvalidArgument, "foo", null, null, null,
            };
            var error = CreateInstance<FirebaseMessagingException>(
                BindingFlags.NonPublic | BindingFlags.Instance,
                types,
                ctorParams);

            return error;
        }

        public static T CreateInstance<T>(
            BindingFlags bindingFlags, Type[] types, object[] ctorParams)
        {
            var type = typeof(T);
            var constructor = type.GetConstructor(
                bindingFlags,
                null,
                types,
                Array.Empty<ParameterModifier>()) ??
                              throw new Exception("Constructor not found.");

            return (T)constructor.Invoke(ctorParams);
        }

        public static List<string> CreateTokens(int count)
        {
            var tokens = new List<string>();
            for (var i = 0; i < count; i++)
            {
                tokens.Add(Guid.NewGuid().ToString());
            }

            return tokens;
        }
    }
}