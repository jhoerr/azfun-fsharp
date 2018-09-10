import * as React from 'react';
import { Route, Switch } from 'react-router-dom';
import Home from './components/Home';
import Page from './components/layout/Page';

const App : React.SFC = () => (
  <Page>
    <Switch>
      <Route path="/" exact={true} component={Home} />
    </Switch>
  </Page>
)

export default App;
