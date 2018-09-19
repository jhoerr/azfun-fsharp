import * as React from 'react';
import { connect } from 'react-redux';
import { Dispatch } from 'redux';
import { IApplicationState } from '../store';
import { profileFetchRequest, profileUpdateRequest } from '../store/profile/actions';
import { IProfileRequest, IProfileState, IProfileUpdateRequest } from '../store/profile/types';
import PageTitle from './layout/PageTitle';
import ProfileForm from './ProfileForm';
import ReadOnlyProfile from './ReadOnlyProfile';

interface IProfileProps {
    match: any,
    path: string
}
// We can use `typeof` here to map our dispatch types to the props, like so.
interface IPropsFromDispatch {
    profileFetchRequest: typeof profileFetchRequest,
    profileUpdateRequest: typeof profileUpdateRequest
}

// tslint:disable-next-line:max-classes-per-file
class ProfileContainer extends React.Component<IProfileState & IProfileProps & IPropsFromDispatch>{

    public isMyProfile() {
        console.log("match path: ", this.props.match.path)
        return this.props.match.path.endsWith('/profile/')
    }

    public componentDidMount() {
        console.log("match params: ", this.props.match.params)
        const id = this.isMyProfile ? 0 : this.props.match.params.id
        this.props.profileFetchRequest({ id })
    }

    public render() {
        return (
            <>
                <PageTitle>Profile</PageTitle>
                { !this.isMyProfile() && 
                    <ReadOnlyProfile {...this.props} />}
                { this.isMyProfile() && 
                    <ProfileForm {...this.props} onSubmit={this.props.profileUpdateRequest} /> }
            </>
        )
    }
}

// Although if necessary, you can always include multiple contexts. Just make sure to
// separate them from each other to prevent prop conflicts.
const mapStateToProps = ({ profile }: IApplicationState) => ({
    ...profile
  })
  
  // mapDispatchToProps is especially useful for constraining our actions to the connected component.
  // You can access these via `this.props`.
  const mapDispatchToProps = (dispatch: Dispatch) : IPropsFromDispatch => ({
    profileFetchRequest: (request: IProfileRequest) => dispatch(profileFetchRequest(request)),
    profileUpdateRequest: (request: IProfileUpdateRequest) => dispatch(profileUpdateRequest(request))
  })
  
// Now let's connect our component!
// With redux v4's improved typings, we can finally omit generics here.
export default connect(
    mapStateToProps,
    mapDispatchToProps
  )(ProfileContainer)
