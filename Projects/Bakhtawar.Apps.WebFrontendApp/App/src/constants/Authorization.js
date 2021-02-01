import { prefixHolder } from '../services/prefix-holder';

export const ApplicationName = 'Bakhtawar';

export const ConfigurationName = 'bakhtawar.web';

export const QueryParameterNames = {
  ReturnUrl: 'returnUrl',
  Message: 'message'
};

export const LogoutActions = {
  LogoutCallback: 'logout-callback',
  Logout: 'logout'
};

export const LoginActions = {
  Login: 'login',
  LoginCallback: 'login-callback',
  LoginFailed: 'login-failed',
  Profile: 'profile',
  Register: 'register'
};

const prefix = '/account';

export const ApplicationPaths = {
  DefaultLoginRedirectPath: '/',
  AuthorizationClientConfigurationURL: `${prefixHolder.Gateway}/_configuration/${ConfigurationName}`,
  AuthorizationPrefix: prefix,
  Login: `${prefix}/${LoginActions.Login}`,
  LoginFailed: `${prefix}/${LoginActions.LoginFailed}`,
  LoginCallback: `${prefix}/${LoginActions.LoginCallback}`,
  Register: `${prefix}/${LoginActions.Register}`,
  Profile: `${prefix}/${LoginActions.Profile}`,
  LogOut: `${prefix}/${LogoutActions.Logout}`,
  LogOutCallback: `${prefix}/${LogoutActions.LogoutCallback}`,
  RegisterPath: '/account/register',
  ManagePath: '/account/manage'
};
