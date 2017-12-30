module Common

open System

[<CLIMutable>]
type Person =
    {
        FirstName : string
        LastName  : string
        BirthDate : DateTime
        Height    : float
        Piercings : string[]
    }
    override this.ToString() =
        let nl = Environment.NewLine
        sprintf "First name: %s%sLast name: %s%sBirth date: %s%sHeight: %.2f%sPiercings: %A"
            this.FirstName nl
            this.LastName nl
            (this.BirthDate.ToString("yyyy-MM-dd")) nl
            this.Height nl
            this.Piercings

