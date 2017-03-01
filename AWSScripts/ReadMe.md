
# AT-T

# AWS Services used

For this game we are only storing the score and player name for displaying on a scoreboard.

We have achieved this using Lambda, API Gateway and DynamoDB.

# API Gateway
This allows exposing functions from services that you have within the AWS ecosystem to eachother or to the world via internet.
We are using this to expose our Lambda functions to https. This does not do much but it is worth mentioning as you'll need to set up any functions 
you add to Lambda in the API Gateway seperatly. Functions that are "published" in Lambda also need to be published in the API Gateway.

# DynamoDB
Stores data in a database, effectively as key-value-pairs. It's very easy to edit and read.

We have two tables, score index table, and Scores table.
ScoreIndex has a single entry key with whose value is an int that represents the current index in the scores table.
The scores table entries are  ID, Name, Score. We use the ID as the index so names do not need to be unique.

The scoreboard functions like an arcade machine, after you finish playing you put in your name and it will be listed sorted first by score then by name.
The ScoreIndex value needs to increased after each update in the score table.


# Lambda 
Lambda allows you to create ad-hoc webserver code, like Node.js, that is scalable and pay-as-you-go. Execution is not instantaneous but it is incredibly cheap.
We are using Lambda as an easy way to access our DynamoDB. In this folder is the code that is used.

There are two Lambda code blocks. One is a pretend POST api and the other just executes a get. 

GetIndex returns the table with the score index. 

In the c# app code we do the following
1) Get current index.
2) Build post request string using the current score, name and index.
3) Send write request string.

ScoreIndex and Scores are updated at the same time when we recieve a correct post request. The score index counter is incrimented 
and the new score value is appended to the Scores table.

You can see the mobile side of this in Assets\Scripts\Mobile\ScoreManager.cs SendScore ln356.


# Resetting the scoreboard

To clear the scoreboard do the following steps. Do this prior to releasing the app.

1) Open DynamoDB in AWS.
2) Select tables
3) Open  the scores table
4) Select all the entries and delete them.
5) Open the indexes table
6) Set the (only) entry value to 1 (So it should be index 0, value 1)







