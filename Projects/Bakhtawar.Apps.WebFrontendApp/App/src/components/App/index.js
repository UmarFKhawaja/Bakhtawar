import React from 'react';
import { BrowserRouter, Route } from 'react-router-dom';
import { Home } from 'components/Home';
import { AuthorizationRoutes } from 'components/AuthorizationRoutes';
import { ContentProvider } from 'components/Content';
import { ApplicationPaths } from 'constants/Authorization';

import './index.css';

export const App = () => (
  <ContentProvider>
    <BrowserRouter>
      <Route exact path='/' component={Home}/>
      <Route path={ApplicationPaths.AuthorizationPrefix} component={AuthorizationRoutes}/>
    </BrowserRouter>
  </ContentProvider>
);