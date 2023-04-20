# Publisher Subscriber Pattern using MessageBroker .netCore
## Problem Statement
One of the Best example Pub Sub pattern can be MessageBroker. So in this example we have tried to create a message broker using .net Core

## Solution Structure

 - MessageBroker(.net Core Api)
 - Sqllite DB
 - TestSubscriber(.net Core Console)


## MessageBroker API description

|API                |Description                         |
|-----------------------|----------------------------------|
|/topic/createtopic| create topic and return topicId |
|/topic/gettopics| Get all topics|
|/topic/PublishTopic| Publish to a topic |
|/subscriptions/Subscribe/<topic id\>| Subscribe to topicId return a unique ID for subscription|
|/subscriptions/GetMessages/<subscription ID \>| Get messages for a id returned in previous subscription |
|/subscriptions/acknowledgemessages| Acknowledge received message|

