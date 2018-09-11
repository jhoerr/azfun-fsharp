import { Reducer } from 'redux'
import { AuthActionTypes, IAuthState } from './types'

// Type-safe initialState!
const initialState: IAuthState = {
  data: "",
  errors: undefined,
  loading: false
}

// Thanks to Redux 4's much simpler typings, we can take away a lot of typings on the reducer side,
// everything will remain type-safe.
const reducer: Reducer<IAuthState> = (state = initialState, action) => {
  switch (action.type) {
    case AuthActionTypes.SIGN_IN: {
      return { ...state, loading: true, data: action.payload }
    }
    case AuthActionTypes.SIGN_IN_SUCCESS: {
      return { ...state, loading: false, data: action.payload }
    }
    case AuthActionTypes.SIGN_IN_ERROR: {
      return { ...state, loading: false, data:"", errors: action.payload }
    }
    case AuthActionTypes.SIGN_OUT: {
        return { ...state, data: "" }
    }
    default: {
      return state
    }
  }
}

// Instead of using default export, we use named exports. That way we can group these exports
// inside the `index.js` folder.
export { reducer as authReducer }
