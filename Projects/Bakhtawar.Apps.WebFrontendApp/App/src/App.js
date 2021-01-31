import React, { Component } from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { Home } from './components/Home';
import { FetchData } from './components/FetchData';
import { Counter } from './components/Counter';
import AuthorizedRoute from './components/api-authorization/AuthorizedRoute';
import ApiAuthorizationRoutes from './components/api-authorization/ApiAuthorizationRoutes';
import { ApplicationPaths } from './components/api-authorization/ApiAuthorizationConstants';
import authService from './components/api-authorization/AuthorizeService'

import './custom.css'

export default class App extends Component {
    static displayName = App.name;

    async componentDidMount() {
        if (this.isSigninRedirectCallback()) {
            await authService.signinRedirectCallback();
            this.navigateHome();
        }
    }

    isSigninRedirectCallback() {
        return window.location.href.includes('code')
            && window.location.href.includes('scope');
    }

    navigateHome() {
        window.location = "/";
    }

    render () {
        return (
            <Layout>
            <Route exact path='/' component={Home} />
            <Route path='/counter' component={Counter} />
            <AuthorizedRoute path='/fetch-data' component={FetchData} />
            <Route path={ApplicationPaths.ApiAuthorizationPrefix} component={ApiAuthorizationRoutes} />
            </Layout>
        );
    }
}
