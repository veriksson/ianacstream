module Types

open System

type User = 
  { Name : string
    Joined : DateTime
    Approved : bool
    StreamKey : string }

type Stream = {
    User : User
    StreamKey : string
    Started : DateTime
    Name :string
}