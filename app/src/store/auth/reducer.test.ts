import * as actions from './actions'
import { authReducer } from './reducer'

it("reduces the sign in request", () => {
    const expectedState = {
        code: "code",
        data: undefined,
        errors: undefined,
        loading: true,
    }

    expect(authReducer(undefined, actions.postSignInRequest({code: "code"})))
        .toEqual(expectedState)
});

it("reduces the success in request", () => {
    const startingState = {
        code: "code",
        data: undefined,
        errors: undefined,
        loading: true,
    }
    const user = {
        user_name: "johndoe", 
        user_role: "admin"
    }
    const expectedState = {
        code: undefined,
        data: user,
        errors: undefined,
        loading: false,
    }

    expect(authReducer(startingState, actions.postSignInSuccess(user)))
        .toEqual(expectedState)
});
