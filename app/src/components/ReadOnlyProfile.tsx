import * as React from 'react'
import { List } from "rivet-react";
import { IProfile } from "../store/profile/types";

const ReadOnlyProfile : React.SFC<IProfile> = 
({ username, preferredName, department, expertise }) => (
        <List>
            <li><strong>Username:</strong> {username}</li>
            <li><strong>Display Name:</strong> {preferredName}</li>
            <li><strong>Department:</strong> {department}</li>
            <li><strong>Expertise:</strong> {expertise}</li>
        </List>
)
export default ReadOnlyProfile
