import React from 'react'
import { Component } from 'react'
import { Route, Redirect } from 'react-router-dom'
import { ApplicationPaths, QueryParameterNames } from '../../constants/Authorization';
import { authorizationManager } from '../../services/authorization-manager';

export default class AuthorizedRoute extends Component {
    constructor(props) {
        super(props);

        this.state = {
            ready: false,
            authenticated: false
        };
    }

    componentDidMount() {
        this._subscription = authorizationManager.subscribe(() => this.authenticationChanged());
        this.populateAuthenticationState().then();
    }

    componentWillUnmount() {
        authorizationManager.unsubscribe(this._subscription);
    }

    render() {
        const { ready, authenticated } = this.state;
        const link = document.createElement("a");

        link.href = this.props.path;

        const returnUrl = `${link.protocol}//${link.host}${link.pathname}${link.search}${link.hash}`;
        const redirectUrl = `${ApplicationPaths.Login}?${QueryParameterNames.ReturnUrl}=${encodeURI(returnUrl)}`

        if (!ready) {
            return <div />;
        } else {
            const { component: Component, ...rest } = this.props;

            return (
                <Route {...rest}
                       render={(props) => {
                           if (authenticated) {
                               return <Component {...props} />
                           } else {
                               return <Redirect to={redirectUrl} />
                           }
                       }}
                />
            );
        }
    }

    async populateAuthenticationState() {
        const authenticated = await authorizationManager.isAuthenticated();
        this.setState({ ready: true, authenticated });
    }

    async authenticationChanged() {
        this.setState({ ready: false, authenticated: false });
        await this.populateAuthenticationState();
    }
}
