import * as React from 'react'
import { Field,InjectedFormProps, reduxForm } from 'redux-form'
import { Button, Form, Input, List } from "rivet-react";
import { IApiState2 } from '../store/common';
import { profileUpdateRequest } from '../store/profile/actions';
import { IProfile } from "../store/profile/types";

interface IProfileFormProps {
    onSubmit: typeof profileUpdateRequest
}

const ix = (props:any) => (
    <Input {...props.input} {...props} />
)

const ProfileForm : React.SFC<IApiState2<IProfile> & IProfileFormProps & InjectedFormProps<{}, IProfile & IProfileFormProps>> = 
({ loading, error, data, onSubmit }) => {
   
    const id = data ? data.id : 0

    const handleSubmit = (e:any)=> {
        e.preventDefault();
        onSubmit({ id });
    }

    return (
            <>
                { !data && loading && <p>Loading...</p>}
                { data &&
                  <>
                    <List>
                        <li><strong>Username:</strong> {data.username}</li>
                        <li><strong>Display Name:</strong> {data.preferredName}</li>
                        <li><strong>Department:</strong> {data.department}</li>
                    </List>
                    <Form  label="Profile update" labelVisibility="screen-reader-only" method="GET" onSubmit={handleSubmit}>
                        <Field type="text" name="expertise" component={ix} label="Expertise" margin={{ bottom: 'md' }}/>        
                        <Button type="submit" disabled={loading}>Submit</Button>
                    </Form>
                  </>
                }
            </>
        )
    
}

export default reduxForm({form:'profile'})(ProfileForm)
