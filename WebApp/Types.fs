module Types

open System

type User = 
  { Name        : string
    Joined      : DateTime
    Approved    : bool
    StreamKey   : string }

type Stream = 
  { StreamKey   : string
    Started     : DateTime
    Name        : string
    Username    : string }