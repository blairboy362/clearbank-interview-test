### Test Description
In the 'PaymentService.cs' file you will find a method for making a payment. At a high level the steps for making a payment are:

 - Lookup the account the payment is being made from
 - Check the account is in a valid state to make the payment
 - Deduct the payment amount from the account's balance and update the account in the database
 
What we’d like you to do is refactor the code with the following things in mind:  
 - Adherence to SOLID principals
 - Testability  
 - Readability 

We’d also like you to add some unit tests to the ClearBank.DeveloperTest.Tests project to show how you would test the code that you’ve produced. The only specific ‘rules’ are:  

 - The solution should build.
 - The tests should all pass.
 - You should not change the method signature of the MakePayment method.

You are free to use any frameworks/NuGet packages that you see fit.  
 
You should plan to spend around 1 to 3 hours to complete the exercise.

### blairboy362 notes

- I used Rider on linux for this implementation.
- Couldn't work out what AccountDataStore vs BackupAccountDataStore were meant to represent, nor why it was going to config to decide what to invoke. So I immediately factored those out behind an interface so I could get my tests to do anything useful.
- Repository, UnitOfWork or similar patterns could be applicable to the stores here.
- Could use different exception types for different error scenarios.
- Loading the account from the store twice seems excessive...not clear why that's being done.
- In my world view, no method returns null. It successfully carries out the task implied by its name, or it throws. So null checks are unnecessary here.
- If this were a proper API contract, I'd be translating between the API types, the datastore types, and, if necessary, a third domain model as things scale in complexity. But that all seems overkill for this.

Assumptions:

- logging, telemetry, tracing, etc. are all out of scope here.
- locking (concurrency etc.) is out of scope.
- all info regarding failure scenarios is swallowed into the single boolean (details would be logged server-side, I'd expect).
- an account can potentially support multiple payment schemes (inferred from the "flag" nature of the enum).
- any kind of ledger is out of scope.
- any kind of audit is out of scope.
- only account status of "live" are allowed to make payments.
- regardless of scheme, balance must remain non negative.
- requested payment date must not be in the past (I'd expect a subject matter expert to define where the line on "past" is drawn; illustrative only in the included tests).
