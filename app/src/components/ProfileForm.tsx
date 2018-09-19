import * as React from 'react'
import { Button, Form, Input, List } from "rivet-react";
import { profileUpdateRequest } from '../store/profile/actions';
import { IProfile } from "../store/profile/types";
import { withLoading } from './util';

interface IProfileFormProps {
    onSubmit: typeof profileUpdateRequest
}

const ProfileForm : React.SFC<IProfile & IProfileFormProps> = 
({ id, username, displayName, department, expertise, onSubmit }) => {

    const getFormValue = (e: React.FormEvent<HTMLFormElement>, name: string) => {
        const element = e.currentTarget.elements.namedItem('expertise') as Element
        console.log("element", element)
        const attr = element.attributes.getNamedItem('value') as Attr
        return attr.value
    }

    const submit = (e: React.FormEvent<HTMLFormElement>) => {
        e.preventDefault()
        console.log(onSubmit)
        onSubmit({id, expertise:getFormValue(e, 'expertise')});
        console.log("submitted")
    }    

    const ignore = () => {;}

    return (
    <>
        <List>
            <li><strong>Username:</strong> {username}</li>
            <li><strong>Display Name:</strong> {displayName}</li>
            <li><strong>Department:</strong> {department}</li>
        </List>
        <Form label="Profile update" labelVisibility="screen-reader-only" method="GET" onSubmit={submit}>
            <Input type="text" name="expertise" label="Expertise" defaultValue={expertise} onChange={ignore} margin={{ bottom: 'md' }} />        
            <Button type="submit">Submit</Button>
        </Form>
    </>
    )
}
export default withLoading<IProfile, IProfileFormProps>(ProfileForm)

