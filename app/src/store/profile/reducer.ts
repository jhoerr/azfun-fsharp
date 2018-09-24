import { Reducer } from 'redux'
import { FetchErrorReducer, FetchRequestReducer, FetchSuccessReducer, PutErrorReducer, PutRequestReducer, PutSuccessReducer } from '../common'
import { IProfileState, ProfileActionTypes } from './types'

// Type-safe initialState!
const initialState: IProfileState = {
    data: undefined,
    error: undefined,
    loading: false,
    request: undefined,
}

// Thanks to Redux 4's much simpler typings, we can take away a lot of typings on the reducer side,
// everything will remain type-safe.
const reducer: Reducer<IProfileState> = (state = initialState, action) => {
  switch (action.type) {
    case ProfileActionTypes.PROFILE_FETCH_REQUEST: return FetchRequestReducer(state, action)
    case ProfileActionTypes.PROFILE_FETCH_SUCCESS: return FetchSuccessReducer(state, action)
    case ProfileActionTypes.PROFILE_FETCH_ERROR: return FetchErrorReducer(state, action)
    case ProfileActionTypes.PROFILE_UPDATE_REQUEST: return PutRequestReducer(state, action)
    case ProfileActionTypes.PROFILE_UPDATE_SUCCESS: return PutSuccessReducer(state, action)
    case ProfileActionTypes.PROFILE_UPDATE_ERROR: return PutErrorReducer(state, action)
    default: return state
  }
}

// Instead of using default export, we use named exports. That way we can group these exports
// inside the `index.js` folder.
export { 
  reducer as profileReducer,
  initialState as initialProfileState
}
