import React from 'react';
import { Container } from 'reactstrap';
import styled from 'styled-components';

const StyledHeader = styled.header.attrs(() => ({
  className: `bg-transparent text-white masthead`
}))`
  padding: 156px 0 100px;
`;

const StyledContainer = styled(Container).attrs(() => ({
  className: 'text-center'
}))``;

const StyledH1 = styled.h1`
  font-size: 2rem;
  text-shadow: 0 0 8px hsl(0, 0%, 0%);
`;

const StyledP = styled.p.attrs(() => ({
  className: 'lead'
}))`
  text-shadow: 0 0 8px hsl(0, 0%, 0%);
`;

export const Header = () => {
  return (
    <StyledHeader>
      <StyledContainer>
        <StyledH1>Bakhtawar Web Frontend</StyledH1>
        <StyledP>React-based web frontend running on .NET 5.0</StyledP>
      </StyledContainer>
    </StyledHeader>
  );
};