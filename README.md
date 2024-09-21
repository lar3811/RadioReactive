An alternative implementation of Reactive Extensions (https://github.com/dotnet/reactive) with slightly different interfaces.

# The Problem:

1. Since IDisposable is returned after Subscribe has completed its execution, it is not possible to unsubscribe until control is returned to calling method.
   Which may not be the case if observable immediately generates infinite or long enough sequence of values (e.g. BehaviourSubject or ReplaySubject).
   For the same reason extension observables like Take, TakeUntil and such may actually go through more values than expected.
2. Observer parameter is mandatory, which in some cases leads to small yet unnecessary overhead of it being repeatedly copied through the stack.
3. Subjects keep references to respective observers in immutable arrays, which forces a new copy to be created each time new observer subscribes.
   Motivation for immutable array is not clear, additional research is required.

# The Solution:

1. Use different subscription model which replaces IDisposable return value with Cancel parameter in Subscribe. Therefore, it should be possible to use the token to interrupt Subscribe execution.
2. Separate set of implementations for parameterless events.
3. Use other collections (LinkedList) and invokation methods (List-index based recursion block).
