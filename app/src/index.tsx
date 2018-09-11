import * as React from 'react';
import * as ReactDOM from 'react-dom';
import { Provider } from 'react-redux';
import { BrowserRouter as Router } from 'react-router-dom';

import 'rivet-uits/css/rivet.min.css';
import App from './App';
import configureStore from './configureStore'
 
const initialState = { auth : { loading: false, data: "", errors: undefined } }
const store = configureStore(initialState)

ReactDOM.render(
  <Provider store={store}>
    <Router>
      <App />
    </Router>
  </Provider>,
  document.getElementById('root'));
