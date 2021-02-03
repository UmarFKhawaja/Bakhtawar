import React from 'react';
import { BrowserRouter, Route } from 'react-router-dom';
import { Layout } from 'components/Layout';
import { Home } from 'components/Home';
import { AuthorizationRoutes } from 'components/AuthorizationRoutes';
import { ContentProvider } from 'components/Content';
import { ApplicationPaths } from 'constants/Authorization';

import './index.css';

export const App = () => (
  <ContentProvider>
    <BrowserRouter>
      <Layout>
        <Route exact path='/'>
          <Home/>
        </Route>
        <Route path={ApplicationPaths.AuthorizationPrefix} component={AuthorizationRoutes}/>
      </Layout>
    </BrowserRouter>
  </ContentProvider>
);