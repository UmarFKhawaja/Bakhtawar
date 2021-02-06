import React from 'react';
import { Route } from 'react-router';
import { ApplicationPaths, LoginActions, LogoutActions } from 'constants/Authorization';
import { Layout } from 'components/Layout';
import { Login } from './components/Login';
import { Logout } from './components/Logout';

export const AuthorizationRoutes = () => (
  <Layout>
    <Route path={ApplicationPaths.Login}><Login action={LoginActions.Login}/></Route>
    <Route path={ApplicationPaths.LoginFailed}><Login action={LoginActions.LoginFailed}/></Route>
    <Route path={ApplicationPaths.LoginCallback}><Login action={LoginActions.LoginCallback}/></Route>
    <Route path={ApplicationPaths.Profile}><Login action={LoginActions.Profile}/></Route>
    <Route path={ApplicationPaths.Register}><Login action={LoginActions.Register}/></Route>
    <Route path={ApplicationPaths.LogOut}><Logout action={LogoutActions.Logout}/></Route>
    <Route path={ApplicationPaths.LogOutCallback}><Logout action={LogoutActions.LogoutCallback}/></Route>
  </Layout>
);