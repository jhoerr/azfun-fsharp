import * as React from 'react'
import { List } from "rivet-react";
import { IProfile } from "../store/profile/types";
import { withLoading } from './util';

const ReadOnlyProfile : React.SFC<IProfile> = 
({ username, displayName, department, expertise }) => (
        <List>
            <li><strong>Username:</strong> {username}</li>
            <li><strong>Display Name:</strong> {displayName}</li>
            <li><strong>Department:</strong> {department}</li>
            <li><strong>Expertise:</strong> {expertise}</li>
        </List>
)
export default withLoading<IProfile, {}>(ReadOnlyProfile)