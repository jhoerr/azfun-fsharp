import { all, call, fork, put, select, takeEvery } from 'redux-saga/effects'
import { callApi } from '../common'
import { signInError, signInSuccess } from './actions'
import { AuthActionTypes   } from './types'

const API_ENDPOINT = process.env.REACT_APP_API_URL || ''

function* handleSignIn() {
  try {
    console.log("handleSignIn...")
    // Get current action state
    const state = yield select();
    console.log("handleSignIn with state", state)
    // To call async functions, use redux-saga's `call()`.
    const res = yield call(callApi, 'get', API_ENDPOINT, `/ping?code=${state.auth.data}`)

    if (res.error) {
      yield put(signInError(res.error))
    } else {
      yield put(signInSuccess(res.token))
    }
  } catch (err) {
    if (err instanceof Error) {
      yield put(signInError(err.stack!))
    } else {
      yield put(signInError('An unknown error occured.'))
    }
  }
}

// This is our watcher function. We use `take*()` functions to watch Redux for a specific action
// type, and run our saga, for example the `handleFetch()` saga above.
function* watchSignIn() {
  console.log("watchSignIn...")
  yield takeEvery(AuthActionTypes.SIGN_IN, handleSignIn)
}

// We can also use `fork()` here to split our saga into multiple watchers.
function* authSaga() {
  yield all([fork(watchSignIn)])
}

export default authSaga
