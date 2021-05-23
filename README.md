# Sample project

### src\ProfanityListServices 
Main services for checking "profanity". 

### src\ChechProfanity 
Main approach for implementation of "profanity service" for AWS.
Contains Lambda functions, AWS deploy script. 
Class ProfanityListDynamoService has many mistakes. I've not fixed it, because I switched to others part of The Task.
Class ProfanityListS3BucketService is not the best way for realization this requirements. But I wanted to learn it :)
I add "serilogger" and log "some information" to CloudWatch.
Http-requerst files may be changed after redeployment.


### src\CheckProfanityKrestel 
Another type of realization endpoints on the _Krestel_ server. But I realized it for some tests and so on.

### test\CheckProfanity.Tests 
Simple tasts

### test\EndpointTest 
http-files for check endpoints with _VSCode REST Client_