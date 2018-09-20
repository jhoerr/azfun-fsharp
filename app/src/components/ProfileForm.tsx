import * as React from 'react'
import { Field,InjectedFormProps, reduxForm } from 'redux-form'
import { Button, Form, List } from "rivet-react";
import { profileUpdateRequest } from '../store/profile/actions';
import { IProfile } from "../store/profile/types";

interface IProfileFormProps {
    onSubmit: typeof profileUpdateRequest
}

const ProfileForm : React.SFC<IProfile & IProfileFormProps & InjectedFormProps<{}, IProfile & IProfileFormProps>> = 
({ id, username, displayName, department, expertise, onSubmit }) => {

   
    const handleSubmit = ()=>{console.log("submit")}

    return (
    <>
        <List>
            <li><strong>Username:</strong> {username}</li>
            <li><strong>Display Name:</strong> {displayName}</li>
            <li><strong>Department:</strong> {department}</li>
        </List>
        <Form label="Profile update" labelVisibility="screen-reader-only" method="GET" onSubmit={handleSubmit}>
            <Field type="text" name="expertise" component="input" label="Expertise" margin={{ bottom: 'md' }} />        
            <Button type="submit">Submit</Button>
        </Form>
    </>
    )
}

const test = reduxForm({
    // a unique name for the form
    form: 'contact'
  })(ProfileForm)
  

export default test

