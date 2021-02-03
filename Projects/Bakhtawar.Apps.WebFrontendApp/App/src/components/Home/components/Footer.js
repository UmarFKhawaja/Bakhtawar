import React from 'react';
import { Container } from 'reactstrap';
import styled from 'styled-components';

const StyledFooter = styled.footer.attrs(() => ({
  className: 'py-5 bg-dark'
}))``;

const Copyright = styled.p.attrs(() => ({
  className: 'm-0 text-center text-white'
}))``;

export const Footer = () => {
  return (
    <StyledFooter>
      <Container>
        <Copyright>Copyright &copy; Bakhtawar 2020-2021</Copyright>
      </Container>
    </StyledFooter>
  );
};