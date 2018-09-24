import * as JWT from 'jwt-decode'
import { push } from 'react-router-redux';
import { all, call, fork, put, select, takeEvery } from 'redux-saga/effects'
import { callApi, clearAuthToken, redirectToLogin, setAuthToken } from '../effects'
import { IApplicationState } from '../index';
import { postSignInError, postSignInSuccess } from './actions'
import { AuthActionTypes, IAuthRequest, IAuthUser } from './types'

const API_ENDPOINT = process.env.REACT_APP_API_URL || ''

function* handleSignIn(){
  yield call(clearAuthToken)
  yield call(redirectToLogin)
}

function* handlePostSignIn() {
  try {
    const request = (yield select<IApplicationState>((s) => s.auth.request)) as IAuthRequest
    const response = yield call(callApi, 'get', API_ENDPOINT, `/auth?code=${request.code}`)

    if (response.errors) {
      yield call(clearAuthToken)
      yield put(postSignInError(response.errors))
    } else {
      yield call(setAuthToken, response.access_token)
      const decoded = JWT<IAuthUser>(response.access_token)
      yield put(postSignInSuccess(decoded))
      yield put(push(`/me`))
    }
  } catch (err) {
    yield call(clearAuthToken)
    if (err instanceof Error) {
      yield put(postSignInError(err.stack!))
    } else {
      yield put(postSignInError('An unknown error occured.'))
    }
  }
}

function* handleSignOut() {
  yield call(clearAuthToken)
  yield put(push('/'))
}

// This is our watcher function. We use `take*()` functions to watch Redux for a specific action
// type, and run our saga, for example the `handleFetch()` saga above.
function* watchSignIn() {
  yield takeEvery(AuthActionTypes.SIGN_IN_REQUEST, handleSignIn)
}

function* watchPostSignIn() {
  yield takeEvery(AuthActionTypes.POST_SIGN_IN_REQUEST, handlePostSignIn)
}

function* watchSignOut() {
  yield takeEvery(AuthActionTypes.SIGN_OUT, handleSignOut)
}

// We can also use `fork()` here to split our saga into multiple watchers.
function* authSaga() {
  yield all([fork(watchSignIn), fork(watchPostSignIn), fork(watchSignOut)])
}

export default authSaga
